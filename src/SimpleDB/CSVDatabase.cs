
using CsvHelper;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Linq.Expressions;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    static string filePath = Path.GetFullPath("bison_observe_cli_db.csv");
    List<int> ids = new List<int>();
    public void setFilePath(string fileType)
    {
        if (fileType == "observation")
        {
            filePath = Path.GetFullPath("bison_observe_cli_db.csv");
        }
        if (fileType == "comment")
        {
            filePath = Path.GetFullPath("bison_comments_cli_db.csv");
        }
        if (fileType == "test")
        {
            filePath = Path.GetFullPath("bison_test.csv");
        }
    }
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
        bool newFile = !File.Exists(filePath);

        using (var writer = new StreamWriter(filePath, true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            if (newFile)//Checks if file exists
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
            }

            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

    public bool doesIdExist(string id)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        var fileInfo = new FileInfo(filePath);

        if (fileInfo.Length == 0)
        {
            return false;
        }

        using var reader = new StreamReader(File.OpenRead(filePath));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            if (csv.GetField("ID") == id)
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerable<Comment> getCommentsUsingId(int id, int? limit = null)
    {
        using (var reader = new StreamReader(File.OpenRead(filePath)))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            return csv.GetRecords<Comment>().Where(c => c.ObservationID == id).ToList();// returns the comment records in a list
        }
    }


    public IEnumerable<Observation> getObservationUsingId(int id, int? limit = null)
    {
        using (var reader = new StreamReader(File.OpenRead(filePath)))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            return csv.GetRecords<Observation>().Where(c => c.ID == id).ToList();// returns the observation records in a list
        }
    }

}

