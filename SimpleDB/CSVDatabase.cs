
using System.Runtime.CompilerServices;
using CsvHelper;
using System.Globalization;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    static string filePath = Path.GetFullPath("bison_observe_cli_db.csv");
    public IEnumerable<T> Read(int? limit = null)
    {
        //Using CSVHelper to handle reading the CSV file
        using (var reader = new StreamReader(File.OpenRead(filePath)))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList();// returns the records in a list
        }
    }

    public void Store(T record)
    {
        //Using CSVHelper to handle writing to CSV file
        using (var writer = new StreamWriter(filePath, true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            if (!File.Exists(filePath))//checks if the filePath exists
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
            }

            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }
}

