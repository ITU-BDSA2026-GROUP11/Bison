public class ObservationService : IObservationService
{
    // Gives this service access to the database
    private readonly DBFacade _db;

    // Gets DBFacade through dependency injection
    public ObservationService(DBFacade db)
    {
        _db = db;
    }

    // Gets all observations from all users
    public List<ObservationViewModel> GetObservations()
    {
        // SQL query that combines observations with their authors
        const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            ORDER BY o.pub_date DESC
            """;

        // Executes the query and turns each database row into an ObservationViewModel
        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ));
    }

    // Gets all observations written by one specific user
    public List<ObservationViewModel> GetObservationsFromAuthor(string author)
    {
        // Same query as above, but only for the given username
        const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY o.pub_date DESC
            """;

        // Adds the author's name safely as a SQL parameter
        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ), command => command.Parameters.AddWithValue("$author", author));
    }

    // Converts a Unix timestamp into a readable date and time
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}