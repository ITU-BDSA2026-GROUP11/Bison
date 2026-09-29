public class ObservationService : IObservationService
{
    private readonly DBFacade _db;

    public ObservationService(DBFacade db)
    {
        _db = db;
    }

    public List<ObservationViewModel> GetObservations()
    {
        const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            ORDER BY o.pub_date DESC
            """;

        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ));
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author)
    {
        const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY o.pub_date DESC
            """;

        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2))
        ), command => command.Parameters.AddWithValue("$author", author));
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}