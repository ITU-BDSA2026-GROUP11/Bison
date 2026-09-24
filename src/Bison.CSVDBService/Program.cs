using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
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
app.Run();
