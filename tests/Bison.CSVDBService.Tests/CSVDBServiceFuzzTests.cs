using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Bison.CSVDBService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SimpleDB;
using Xunit.Abstractions;

namespace Bison.CSVDBService.Tests;

/// <summary>
/// Fuzz-based E2E test (task 4.1.e).
/// Sends a random sequence of observations, comments and proposals to the service,
/// keeps an in-memory oracle of what *should* be stored, and regularly checks that
/// /observations, /comments and /proposals return exactly what the oracle predicts.
///
/// Most generated posts use valid observation/taxon IDs (see ValidIdRate) so the fuzzer
/// doesn't get stuck sending only invalid posts (the "fuzz blocker" from the assignment).
///
/// Every run uses a random seed that is printed in the test output. To reproduce a failure:
///   $env:BISON_FUZZ_SEED=12345; dotnet test
/// </summary>
public sealed class CSVDBServiceFuzzTests : IDisposable
{
    // Flip to true once task 1.d (/proposal and /proposals) is merged into main.
    private static readonly bool ProposalsImplemented = false;

    private const int Operations = 300;      // number of random posts per run
    private const int VerifyEvery = 25;      // compare service against oracle this often
    private const double ValidIdRate = 0.85; // share of comments/proposals that use valid IDs

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static readonly string[] Authors = { "Peter", "Signe", "Jónas", "Ása", "Mads", "Freja" };

    // Includes commas, quotes and Danish letters on purpose: they stress the CSV layer.
    private const string Alphabet =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZæøåÆØÅ0123456789 ,.;:'\"!?-_()";

    private readonly ITestOutputHelper output;
    private readonly string dataDir;
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public CSVDBServiceFuzzTests(ITestOutputHelper output)
    {
        this.output = output;

        // Fresh, empty CSV files for every run, so the real CLI data is never touched.
        dataDir = Path.Combine(Path.GetTempPath(), $"bison-fuzz-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dataDir);

        factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Database:Observations", Path.Combine(dataDir, "observations.csv"));
            builder.UseSetting("Database:Comments", Path.Combine(dataDir, "comments.csv"));
            builder.UseSetting("Database:Proposals", Path.Combine(dataDir, "proposals.csv"));
        });

        client = factory.CreateClient();
    }

    [Fact]
    public async Task RandomPostsMatchOracle()
    {
        int seed = int.TryParse(Environment.GetEnvironmentVariable("BISON_FUZZ_SEED"), out var fixedSeed)
            ? fixedSeed
            : Random.Shared.Next();
        output.WriteLine($"Fuzz seed: {seed}. Reproduce with: $env:BISON_FUZZ_SEED={seed}; dotnet test");

        var rng = new Random(seed);
        var oracle = new Oracle();

        HashSet<string> taxonIds = ProposalsImplemented
            ? factory.Services.GetRequiredService<ITaxonomyRepository>()
                .GetAll()
                .Select(t => t.TaxonId)
                .ToHashSet()
            : new();

        for (int step = 1; step <= Operations; step++)
        {
            double roll = rng.NextDouble();

            if (roll < 0.40)
                await PostObservation(rng, oracle);
            else if (!ProposalsImplemented || roll < 0.75)
                await PostComment(rng, oracle);
            else
                await PostProposal(rng, oracle, taxonIds);

            if (step % VerifyEvery == 0)
            {
                output.WriteLine($"Verifying after step {step} " +
                    $"({oracle.Observations.Count} observations, {oracle.Comments.Count} comments, " +
                    $"{oracle.Proposals.Count} proposals expected)");
                await Verify(rng, oracle);
            }
        }
    }

    // ---------------------------------------------------------------- Posting

    private async Task PostObservation(Random rng, Oracle oracle)
    {
        var observation = new Observation
        {
            Author = RandomAuthor(rng),
            ObservationText = RandomText(rng, 60),
            Location = RandomText(rng, 20),
            Timestamp = rng.NextInt64(0, 2_000_000_000), // the service overwrites this, so we never compare it
        };

        var response = await client.PostAsJsonAsync("/observation", observation, Json);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // The service hands out IDs as max + 1, starting from 1 in an empty database.
        oracle.Observations.Add(observation with { ID = oracle.Observations.Count + 1 });
    }

    private async Task PostComment(Random rng, Oracle oracle)
    {
        var comment = new Comment
        {
            Author = RandomAuthor(rng),
            ObservationText = RandomText(rng, 60),
            ObservationID = PickObservationId(rng, oracle),
        };

        var response = await client.PostAsJsonAsync("/comment", comment, Json);

        if (oracle.ObservationExists(comment.ObservationID))
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            oracle.Comments.Add(comment);
        }
        else
        {
            // The assignment doesn't fix a status code for invalid posts, only that they
            // must not be stored. A 5xx would mean the service crashed, which is always a bug.
            Assert.True((int)response.StatusCode < 500,
                $"Comment on missing observation {comment.ObservationID} crashed the service: {response.StatusCode}");
        }
    }

    private async Task PostProposal(Random rng, Oracle oracle, HashSet<string> taxonIds)
    {
        var proposal = new ProposalDto
        {
            Author = RandomAuthor(rng),
            ObservationID = PickObservationId(rng, oracle),
            TaxonId = PickTaxonId(rng, taxonIds),
        };

        var response = await client.PostAsJsonAsync("/proposal", proposal, Json);

        // An invalid taxon ID is treated exactly like an invalid observation ID.
        bool valid = oracle.ObservationExists(proposal.ObservationID) && taxonIds.Contains(proposal.TaxonId);

        if (valid)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            oracle.Proposals.Add(proposal);
        }
        else
        {
            Assert.True((int)response.StatusCode < 500,
                $"Invalid proposal (obs {proposal.ObservationID}, taxon '{proposal.TaxonId}') " +
                $"crashed the service: {response.StatusCode}");
        }
    }

    // ---------------------------------------------------------------- Verifying

    private async Task Verify(Random rng, Oracle oracle)
    {
        // /observations returns everything, in insertion order.
        var observations = await client.GetFromJsonAsync<List<Observation>>("/observations", Json);
        Assert.NotNull(observations);
        Assert.Equal(
            oracle.Observations.Select(ObservationKey),
            observations!.Select(ObservationKey));

        // /comments and /proposals return only the posts for the requested observation.
        foreach (var observation in oracle.Observations)
        {
            var comments = await client.GetFromJsonAsync<List<Comment>>($"/comments?id={observation.ID}", Json);
            Assert.NotNull(comments);
            Assert.Equal(
                oracle.Comments.Where(c => c.ObservationID == observation.ID).Select(CommentKey),
                comments!.Select(CommentKey));

            if (ProposalsImplemented)
            {
                var proposals = await client.GetFromJsonAsync<List<ProposalDto>>($"/proposals?id={observation.ID}", Json);
                Assert.NotNull(proposals);
                Assert.Equal(
                    oracle.Proposals.Where(p => p.ObservationID == observation.ID).Select(ProposalKey),
                    proposals!.Select(ProposalKey));
            }
        }

        // Asking for an observation that doesn't exist must never return posts.
        int missingId = oracle.Observations.Count + rng.Next(1, 1000);
        await AssertNoPostsReturned($"/comments?id={missingId}");
        if (ProposalsImplemented)
            await AssertNoPostsReturned($"/proposals?id={missingId}");
    }

    private async Task AssertNoPostsReturned(string url)
    {
        var response = await client.GetAsync(url);
        Assert.True((int)response.StatusCode < 500, $"GET {url} crashed the service: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
            return; // 400 or 404 are both reasonable answers

        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return;

        var items = JsonSerializer.Deserialize<List<JsonElement>>(body, Json);
        Assert.True(items is null || items.Count == 0,
            $"GET {url} returned posts for an observation that doesn't exist: {body}");
    }

    // What we compare. Timestamps and comment/proposal IDs are assigned by the service, so we skip them.
    private static (int, string, string, string) ObservationKey(Observation o) =>
        (o.ID, o.Author, o.ObservationText, o.Location);

    private static (int, string, string) CommentKey(Comment c) =>
        (c.ObservationID, c.Author, c.ObservationText);

    private static (int, string, string) ProposalKey(ProposalDto p) =>
        (p.ObservationID, p.Author, p.TaxonId);

    // ---------------------------------------------------------------- Generators

    private static int PickObservationId(Random rng, Oracle oracle)
    {
        if (oracle.Observations.Count > 0 && rng.NextDouble() < ValidIdRate)
            return rng.Next(1, oracle.Observations.Count + 1);

        return rng.Next(3) switch
        {
            0 => 0,
            1 => -rng.Next(1, 1000),
            _ => oracle.Observations.Count + rng.Next(1, 1000),
        };
    }

    private static string PickTaxonId(Random rng, HashSet<string> taxonIds)
    {
        if (taxonIds.Count > 0 && rng.NextDouble() < ValidIdRate)
            return taxonIds.ElementAt(rng.Next(taxonIds.Count));

        return rng.Next(3) switch
        {
            0 => "",
            1 => "not-a-taxon",
            _ => RandomText(rng, 12),
        };
    }

    private static string RandomAuthor(Random rng) =>
        rng.NextDouble() < 0.8 ? Authors[rng.Next(Authors.Length)] : RandomText(rng, 15);

    private static string RandomText(Random rng, int maxLength)
    {
        int length = rng.Next(0, maxLength + 1);
        var text = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            double roll = rng.NextDouble();
            if (roll < 0.02)
                text.Append('\n');      // line breaks inside a CSV field
            else if (roll < 0.03)
                text.Append("🐦");      // non-BMP character (surrogate pair)
            else
                text.Append(Alphabet[rng.Next(Alphabet.Length)]);
        }

        return text.ToString();
    }

    // ---------------------------------------------------------------- Oracle

    private sealed class Oracle
    {
        public List<Observation> Observations { get; } = new();
        public List<Comment> Comments { get; } = new();
        public List<ProposalDto> Proposals { get; } = new();

        public bool ObservationExists(int id) => id >= 1 && id <= Observations.Count;
    }

    // Placeholder until 1.d adds a real Proposal type. Swap it for that type once it exists.
    private sealed record ProposalDto
    {
        public int ID { get; set; }
        public string Author { get; set; } = "";
        public int ObservationID { get; set; }
        public string TaxonId { get; set; } = "";
        public long Timestamp { get; set; }
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
        try { Directory.Delete(dataDir, recursive: true); } catch (IOException) { }
    }
}
