namespace Bison.Razor.Repositories;

// Handles all database access for posts.
// Only the DI container (Program.cs) knows about this class.
public class PostRepository : IPostRepository
{
    // Gives the repository access to the database
    private readonly DBFacade _db;

    // Gets DBFacade through dependency injection
    public PostRepository(DBFacade db)
    {
        _db = db;
    }

    public List<ObservationViewModel> GetObservations(int page, int pageSize)
    {
        // Calculates how many observations should be skipped
        int offset = (page - 1) * pageSize;

        const string sql = """
            SELECT u.username, o.text, o.pub_date, o.observation_id
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            ORDER BY o.pub_date DESC, o.observation_id DESC
            LIMIT $pageSize OFFSET $offset
            """;

        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2)),
            reader.GetInt32(3)
        ), command =>
        {
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$offset", offset);
        });
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page, int pageSize)
    {
        // Calculates how many observations should be skipped
        int offset = (page - 1) * pageSize;

        const string sql = """
            SELECT u.username, o.text, o.pub_date, o.observation_id
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY o.pub_date DESC, o.observation_id DESC
            LIMIT $pageSize OFFSET $offset
            """;

        return _db.ExecuteQuery(sql, reader => new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2)),
            reader.GetInt32(3)
        ), command =>
        {
            command.Parameters.AddWithValue("$author", author);
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$offset", offset);
        });
    }

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
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}