namespace Bison.Razor;


public class ObservationService : IObservationService
{
    // Gives this service access to the database
    private readonly DBFacade _db;

    // Amount of observations each page holds
    private const int PageSize = 32;

    // Gets DBFacade through dependency injection
    public ObservationService(DBFacade db)
    {
        _db = db;
    }

    // Gets observations from all users for a specific page
    public List<ObservationViewModel> GetObservations(int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        // Calculates how many observations should be skipped
        int offset = (page - 1) * PageSize;

        // Gets observations and their authors, ordered from newest to oldest
        const string sql = """
            SELECT u.username, o.text, o.pub_date, o.observation_id
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            ORDER BY o.pub_date DESC, o.observation_id DESC
            LIMIT $pageSize OFFSET $offset
            """;

        // Executes the query and converts each row into an ObservationViewModel
        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2)),
            reader.GetInt32(3)
        ), command =>
        {
            // Sets the maximum amount of observations returned
            command.Parameters.AddWithValue("$pageSize", PageSize);

            // Skips observations belonging to earlier pages
            command.Parameters.AddWithValue("$offset", offset);
        });
    }

    // Gets observations written by one specific user for a specific page
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        // Calculates how many observations should be skipped
        int offset = (page - 1) * PageSize;

        // Gets observations only from the given username
        const string sql = """
            SELECT u.username, o.text, o.pub_date, o.observation_id
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY o.pub_date DESC, o.observation_id DESC
            LIMIT $pageSize OFFSET $offset
            """;

        // Executes the query and converts each row into an ObservationViewModel
        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2)),
            reader.GetInt32(3)
        ), command =>
        {
            // Specifies which author's observations should be returned
            command.Parameters.AddWithValue("$author", author);

            // Sets the maximum amount of observations returned
            command.Parameters.AddWithValue("$pageSize", PageSize);

            // Skips observations belonging to earlier pages
            command.Parameters.AddWithValue("$offset", offset);
        });
    }

        // Gets one observation and its author
    public ObservationViewModel? GetObservation(int id)
    {
        const string sql = """
            SELECT o.observation_id, u.username, o.text, o.pub_date
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            WHERE o.observation_id = $id
            """;

        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(1),
            reader.GetString(2),
            UnixTimeStampToDateTimeString(reader.GetInt64(3)),
            reader.GetInt32(0)
        ), command => command.Parameters.AddWithValue("$id", id))
        .FirstOrDefault();
    }

    // Gets all comments on an observation, oldest first, so they read like a conversation
    public List<CommentViewModel> GetComments(int observationId)
    {
        const string sql = """
            SELECT u.username, c.text, c.pub_date
            FROM comment AS c
            JOIN user AS u ON c.author_id = u.user_id
            WHERE c.observation_id = $observationId
            ORDER BY c.pub_date ASC, c.comment_id ASC
            """;

        return _db.ExecuteQuery(sql, reader => new CommentViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ), command => command.Parameters.AddWithValue("$observationId", observationId));
    }

    // Gets all taxon proposals on an observation, oldest first
    public List<ProposalViewModel> GetProposals(int observationId)
    {
        const string sql = """
            SELECT u.username, p.taxon_id, p.pub_date
            FROM proposal AS p
            JOIN user AS u ON p.author_id = u.user_id
            WHERE p.observation_id = $observationId
            ORDER BY p.pub_date ASC, p.proposal_id ASC
            """;

        return _db.ExecuteQuery(sql, reader => new ProposalViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ), command => command.Parameters.AddWithValue("$observationId", observationId));
    }





    // Converts a Unix timestamp into a readable date and time
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Starts at the Unix epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        // Adds the timestamp seconds to get the correct date
        dateTime = dateTime.AddSeconds(unixTimeStamp);

        // Returns the date as a formatted string
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}