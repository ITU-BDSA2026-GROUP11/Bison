
using Bison.Infastructure;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Bison.E2ETests;

[TestFixture]
[NonParallelizable]
public class ObservationE2ETests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("BISON_E2E_BASE_URL")
        ?? "http://127.0.0.1:5005";

    private static string DatabasePath =>
        Environment.GetEnvironmentVariable("BISON_E2E_DB")
        ?? throw new InvalidOperationException(
            "BISON_E2E_DB must point to the test database.");

    // Change this if your observation form is on another page.
    private const string ObservationFormPath = "/obs";

    // Taxon 26 is one of Bison's seeded taxa.
    private const int TestTaxonId = 26;

    [OneTimeSetUp]
    public void CheckTestConfiguration()
    {
        var host = new Uri(BaseUrl).Host;

        if (host != "localhost" && host != "127.0.0.1")
        {
            throw new InvalidOperationException(
                "E2E tests must run against a local server.");
        }

        if (!Path.GetFileName(DatabasePath)
                .Contains("e2e", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Use a separate database file containing 'e2e' in its name.");
        }
    }

    private BisonDBContext OpenDatabase()
    {
        var options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite($"Data Source={DatabasePath}")
            .Options;

        return new BisonDBContext(options);
    }

    // Register a different user for each test.
    private async Task<(string Email, string DisplayName)> RegisterUser()
    {
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        var email = $"e2e-{uniqueId}@example.test";
        var displayName = $"E2ETester{uniqueId}";

        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Register");

        await Page.Locator("#Input_DisplayName")
            .FillAsync(displayName);

        await Page.Locator("#Input_Email")
            .FillAsync(email);

        await Page.Locator("#Input_Password")
            .FillAsync("TestPassword!123");

        await Page.Locator("#Input_ConfirmPassword")
            .FillAsync("TestPassword!123");

        await Page.Locator("#registerForm button[type='submit']")
            .ClickAsync();

        // Registration should leave the new user signed in.
        await Expect(
            Page.GetByRole(AriaRole.Button,
                new() { Name = "Logout", Exact = false })
        ).ToBeVisibleAsync();

        return (email, displayName);
    }

    // TEST 1: Observation is saved for the correct author.
    [Test]
    public async Task NewObservation_IsStoredForCorrectAuthor()
    {
        var (email, displayName) = await RegisterUser();

        var observationText =
            $"E2E bird observation {Guid.NewGuid():N}";

        // Interact with the real observation form.
        await Page.GotoAsync($"{BaseUrl}{ObservationFormPath}");

        await Page.GetByTestId("observation-text")
            .FillAsync(observationText);

        await Page.GetByTestId("observation-taxon")
            .SelectOptionAsync(TestTaxonId.ToString());

        await Page.GetByTestId("observation-submit")
            .ClickAsync();

        // Verify the observation through Entity Framework.
        await using var db = OpenDatabase();

        var savedObservation = await db.Observations
            .Include(o => o.Author)
            .SingleOrDefaultAsync(o => o.Text == observationText);

        Assert.That(savedObservation, Is.Not.Null,
            "Observation was not found in the database.");

        Assert.Multiple(() =>
        {
            Assert.That(savedObservation!.Author.Email,
                Is.EqualTo(email).IgnoreCase);

            Assert.That(savedObservation.Author.Name,
                Is.EqualTo(displayName));

            Assert.That(savedObservation.AuthorId,
                Is.EqualTo(savedObservation.Author.Id));

            Assert.That(savedObservation.TaxonId,
                Is.EqualTo(TestTaxonId));
        });

        // Verify it is also visible on the public timeline.
        await Page.GotoAsync($"{BaseUrl}/obs");

        await Expect(Page.GetByText(observationText))
            .ToBeVisibleAsync();
    }

    // TEST 2: Comment is stored for the correct author.
    [Test]
    public async Task NewComment_IsStoredForCorrectAuthor()
    {
        var (email, _) = await RegisterUser();

        var commentText =
            $"E2E comment {Guid.NewGuid():N}";

        // Observation 1 is created by DbInitializer.
        await Page.GotoAsync($"{BaseUrl}/ob/1");

        await Page.Locator("input[name='CommentText']")
            .FillAsync(commentText);

        await Page.Locator(
            "input[type='submit'][value='Comment']")
            .ClickAsync();

        await Expect(Page.GetByText(commentText))
            .ToBeVisibleAsync();

        await using var db = OpenDatabase();

        var savedComment = await db.Comments
            .Include(c => c.Author)
            .SingleOrDefaultAsync(c => c.Text == commentText);

        Assert.That(savedComment, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(savedComment!.ObservationId,
                Is.EqualTo(1));

            Assert.That(savedComment.Author.Email,
                Is.EqualTo(email).IgnoreCase);

            Assert.That(savedComment.AuthorId,
                Is.EqualTo(savedComment.Author.Id));
        });
    }

    // TEST 3: Visitors cannot submit comments.
    [Test]
    public async Task LoggedOutUser_CannotPostComment()
    {
        await Page.GotoAsync($"{BaseUrl}/ob/1");

        // The comment form should be hidden when logged out.
        var commentInput =
            Page.Locator("input[name='CommentText']");

        Assert.That(await commentInput.CountAsync(),
            Is.EqualTo(0));

        await Expect(
            Page.GetByRole(AriaRole.Link,
                new() { Name = "Log in", Exact = true })
                .Last
        ).ToBeVisibleAsync();
    }

    // TEST 4: Axe accessibility checks.
    [TestCase("/obs")]
    [TestCase("/ob/1")]
    [TestCase("/Identity/Account/Register")]
    public async Task Page_HasNoAxeViolations(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");

        AxeResult result = await Page.RunAxe();

        var errors = string.Join(
            Environment.NewLine,
            result.Violations.Select(v =>
                $"{v.Id}: {v.Help}"));

        Assert.That(
            result.Violations,
            Is.Null.Or.Empty,
            errors);
    }
}