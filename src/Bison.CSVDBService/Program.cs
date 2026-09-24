using SimpleDB;
using Bison.CSVDBService;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaxonomyRepository, TaxonomyRepository>();

var app = builder.Build();
var obs_db = CSVDatabase<Observation>.getInstance("../Bison.CLI/bison_observe_cli_db.csv");
var evil_db = CSVDatabase<Comment>.getInstance("../Bison.CLI/bison_comments_cli_db.csv");
app.MapPost("/observation", (Observation observation) => 
{
var observations = obs_db.Read();

    

    int nextID = observations.Any()
        ? observations.Max(o => o.ID) + 1
        : 1;

    observation.ID = nextID;

    Observation observationNew = new()
    {
        ID = nextID,
        Author = observation.Author,
        ObservationText = observation.ObservationText,
        Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
    };

    obs_db.Store(observationNew);
//Implement three endpoints in your CSV database web service: 
// /observation, /comment, /comments and /observations. Sending a JSON object, e.g., 
// of the form {"Author":"Peter","Message":"Another Heron", "Timestamp": 1684229348} 
// as the body of an HTTP POST request to the /observation endpoint shall store the 
// post in the remote database.
});

app.MapPost("/comment", (Comment comment) => {
    //  The same holds for /comment and /comments.

    if (!obs_db.Read().Any(observation => observation.ID == comment.ObservationID))
    {
        var comments = evil_db.Read();
            int nextID = comments.Any()
                ? comments.Max(c => c.ID) + 1
                : 1;

        
        Comment commentNew = new()
        {
            ObservationID = comment.ObservationID,
            Author = comment.Author,
            ObservationText = comment.ObservationText,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            ID = nextID

        };
         evil_db.Store(commentNew);
    }

});

app.MapGet("/comments", (int id) => {
    // An HTTP GET request to the /comments endpoint shall
    // contain an id of an observation and return all comments for that observation
    // that are stored in the CSV database as a list of JSON objects.

    if(!obs_db.Read().Any(observation => observation.ID == id))
    {
        return null;
    }
    return evil_db.Read();
});


app.MapGet("/observations", () =>
{
    var observations = obs_db.Read();
    return observations;
    //  An HTTP GET request to the /observations 
    // endpoint shall return all observations that are stored in the CSV database as a 
    // list of JSON objects
});
// temp test for the taxonomy
// use "dotnet run" while in Bison.CSVDBService
using var scope = app.Services.CreateScope();

var repo = scope.ServiceProvider.GetRequiredService<ITaxonomyRepository>();

Console.WriteLine("Testing taxonomy repository...");

var fiskehejre = repo.GetByVernacularName("Fiskehejre");

if (fiskehejre == null)
{
    Console.WriteLine("Could not find Fiskehejre");
}
else
{
    Console.WriteLine($"Found: {fiskehejre.ScientificName}");

    var parent = repo.GetSupertaxon(fiskehejre.TaxonId);

    if (parent != null)
    {
        Console.WriteLine($"Parent: {parent.ScientificName}");

        var children = repo.GetSubtaxons(parent.TaxonId);
        Console.WriteLine($"Number of subtaxons: {children.Count}");
    }
    else
    {
        Console.WriteLine("No parent found");
    }
}

app.Run();

