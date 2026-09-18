using CsvHelper;
using System.Globalization;

namespace SimpleDB;

public sealed class CSVDatabase<T>: IDatabaseRepository<T>
{
    // Singleton instance
    private static CSVDatabase<T>? instance;

    // Paths to the two CSV files
    private readonly string FilePath;

    // Private constructor means nobody outside this class
    // can create a new CSVDatabase object
    private CSVDatabase(string FilePath)
    {
        this.FilePath = Path.GetFullPath(FilePath);
    }

    // The only way to access the database
    public static CSVDatabase<T> getInstance(String FilePath)
    {
        instance ??= new CSVDatabase<T>(FilePath);
        return instance;
    }

    // Observations----------------------------------------------------
    public IEnumerable<T> Read(int? limit = null)
    {
        return ReadFromFile<T>(
            FilePath,
            limit
        );
    }

    public void Store(T record)
    {
        StoreToFile(
            FilePath,
            record
        );
    }

    //public bool DoesObservationIdExist(int id)
    //{
    //    return ReadObservations()
    //        .Any(observation => observation.ID == id);
    //}

    //public IEnumerable<Observation> GetObservationUsingId(int id)
    //{
    //    return ReadObservations()
    //        .Where(observation => observation.ID == id);
    //}


    // Comments----------------------------------------------------

    //public IEnumerable<Comment> ReadComments(int? limit = null)
    //{
    //    return ReadFromFile<Comment>(
    //        commentFilePath,
    //        limit
    //    );
    //}

    //public void StoreComment(Comment comment)
    //{
    //    StoreToFile(
    //        commentFilePath,
    //        comment
    //    );
    //}

    //public IEnumerable<Comment> GetCommentsUsingId(int observationId)
    //{
    //    return ReadComments()
    //        .Where(comment =>
    //            comment.ObservationID == observationId
    //        );
    //}

    // Generic CSV handling----------------------------------------------------

    private IEnumerable<T> ReadFromFile<T>(
        string filePath,
        int? limit = null)
    {
        if (!File.Exists(filePath))
        {
            return Enumerable.Empty<T>();
        }

        // Also handle an existing but empty file
        FileInfo fileInfo = new(filePath);

        if (fileInfo.Length == 0)
        {
            return Enumerable.Empty<T>();
        }

        using var reader =
            new StreamReader(filePath);

        using var csv =
            new CsvReader(
                reader,
                CultureInfo.InvariantCulture
            );

        var records =
            csv.GetRecords<T>().ToList();

        if (limit.HasValue)
        {
            return records.Take(limit.Value);
        }

        return records;
    }

    private void StoreToFile<T>(
        string filePath,
        T record)
    {
        bool needsHeader =
            !File.Exists(filePath)
            || new FileInfo(filePath).Length == 0;

        using var writer =
            new StreamWriter(
                filePath,
                append: true
            );

        using var csv =
            new CsvWriter(
                writer,
                CultureInfo.InvariantCulture
            );

        if (needsHeader)
        {
            csv.WriteHeader<T>();
            csv.NextRecord();
        }

        csv.WriteRecord(record);
        csv.NextRecord();
    }

    //public IEnumerable<Observation> GetObservationsByLocation(string location)
    //{
    //    return ReadObservations()
    //        .Where(observation =>
    //            observation.Location.Equals(
    //                location,
    //                StringComparison.OrdinalIgnoreCase
    //            )
    //        );
    //}

}