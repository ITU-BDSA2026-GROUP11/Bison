using Bison.Razor;

namespace Bison.Razor.Tests;

public class BisonIntegrationTests
{
    
    private readonly DBFacade _db_test;
    private readonly string _pathTemp;

    //private List<T> results;

    public BisonIntegrationTests()
    {
        _pathTemp = Path.GetTempFileName();
        _db_test = new DBFacade(_pathTemp);
    }
    
    [Fact]
    public void DBFacadeReturnsQuery()
    {
        const string sqlInsert = """
        INSERT INTO observation VALUES(501, 1, "Test observation", 9999999)
        """;
        // Given
    
       const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation AS o
            JOIN user AS u ON o.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY o.pub_date DESC, o.observation_id DESC
            """;
        // When
        //results = _db_test.ExecuteQuery(sql, reader => new ObservationViewModel(),)
        // Then
        
    }
}