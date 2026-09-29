using Microsoft.Data.Sqlite;

public class DBFacade
{
    private readonly string _connectionString;

    public DBFacade(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
    }

    public List<T> ExecuteQuery<T>(string sql, Func<SqliteDataReader, T> mapper, Action<SqliteCommand>? addParameters = null)
    {
        List<T> result = new List<T>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        addParameters?.Invoke(command);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            result.Add(mapper(reader));
        }

        return result;
    }
}