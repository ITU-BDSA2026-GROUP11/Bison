public class ObservationService : IObservationService
{
    // Gives this service access to the database
    private readonly DBFacade _db;

    // Amount of Observations each page holds
    private const int PageSize = 32;

    // Gets DBFacade through dependency injection
    public ObservationService(DBFacade db)
    {
        _db = db;

    }

    // Gets all observations from all users
    public List<ObservationViewModel> GetObservations(int page = 1)
    {
        if (page < 1)
        {
            page = 1;
        }

        int offset = (page - 1) * PageSize;

        const string sql = """
        SELECT u.username, o.text, o.pub_date
        FROM observation AS o
        JOIN user AS u ON o.author_id = u.user_id
        ORDER BY o.pub_date DESC, o.observation_id DESC
        LIMIT $pageSize OFFSET $offset
        """;

        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ), command =>
        {
            // Request the amount of observation corresponding to the pageSize value
            command.Parameters.AddWithValue("$pageSize", PageSize);
            // Skips observations belonging to earlier pages
            command.Parameters.AddWithValue("$offset", offset);
        });
    }

    // Gets all observations written by one specific user
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 1)
    {
        if (page < 1)
        {
            page = 1;
        }

        int offset = (page - 1) * PageSize;

        // Same query as above, but only for the given username
        const string sql = """
        SELECT u.username, o.text, o.pub_date
        FROM observation AS o
        JOIN user AS u ON o.author_id = u.user_id
        WHERE u.username = $author
        ORDER BY o.pub_date DESC, o.observation_id DESC
        LIMIT $pageSize OFFSET $offset
        """;

        // Adds the author's name safely as a SQL parameter
        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ), command =>
        {
            command.Parameters.AddWithValue("$author", author);
            command.Parameters.AddWithValue("$pageSize", PageSize);
            command.Parameters.AddWithValue("$offset", offset);
        });
    }

    // Converts a Unix timestamp into a readable date and time
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}