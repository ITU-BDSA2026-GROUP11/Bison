using Microsoft.Data.Sqlite;

public class DBFacade
{
    // Stores the information needed to connect to the database
    private readonly string _connectionString;

    // Creates the connection string from the database file path
    public DBFacade(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
    }

    // Runs a SQL query and returns the result as a list
    public List<T> ExecuteQuery<T>(string sql, Func<SqliteDataReader, T> mapper, Action<SqliteCommand>? addParameters = null)
    {
        // List that will contain the data returned from the database
        List<T> result = new List<T>();

        // Opens a connection to the SQLite database
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Creates the SQL command
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // Adds SQL parameters if the query needs any
        addParameters?.Invoke(command);

        // Runs the query and reads the returned rows
        using var reader = command.ExecuteReader();

        // Converts each database row into the wanted object
        while (reader.Read())
        {
            result.Add(mapper(reader));
        }

        // Returns all the objects created from the database rows
        return result;
    }
}