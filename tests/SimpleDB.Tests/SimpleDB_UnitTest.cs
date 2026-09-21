using SimpleDB;

namespace SimpleDB.Tests;

public class SimpleDB_UnitTest : IDisposable
{
    private readonly CSVDatabase<Observation> db;
    private readonly string observationFilePath;

    public SimpleDB_UnitTest()
    {
        

        observationFilePath =
            Path.GetFullPath("bison_observe_cli_db.csv");

        // Start each test with a clean observation database
        if (File.Exists(observationFilePath))
        {
            File.Delete(observationFilePath);
        }
        db = CSVDatabase<Observation>.getInstance("bison_observe_cli_db.csv");
    }

    [Fact]
    public void DoesObservationIdExist_ReturnsFalse_ForNonExistingId()
    {
        // Arrange
        File.WriteAllText(
            observationFilePath,
            "Author,ObservationText,Timestamp,ID,Location\n" +
            "Oliver,Penguin,12345,1,location x\n"
        );

        // Act
        bool result = db.Read().Any(observation => observation.ID == 999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesObservationIdExist_ReturnsTrue_ForExistingId()
    {
        // Arrange
        File.WriteAllText(
            observationFilePath,
            "Author,ObservationText,Timestamp,ID,Location\n" +
            "Oliver,Penguin,12345,1,location x\n"
        );

        // Act
        bool result = db.Read().Any(observation => observation.ID == 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void StoreObservation_Then_ReadObservations_ReturnsStoredObservation()
    {
        // Arrange
        var observation = new Observation
        {
            Author = "Oliver",
            ObservationText = "Penguin",
            Timestamp = 12345,
            ID = 1,
            Location = "location x"
        };

        // Act
        db.Store(observation);

        var result =
            db.Read().ToList();

        // Assert
        Assert.Single(result);

        Assert.Equal(
            "Penguin",
            result[0].ObservationText
        );

        Assert.Equal(
            "Oliver",
            result[0].Author
        );

        Assert.Equal(
            1,
            result[0].ID
        );
        
        Assert.Equal(
            "location x",
            result[0].Location
        );
    }

    [Fact]
    public void GetObservationUsingId_ReturnsCorrectObservation()
    {
        // Arrange
        File.WriteAllText(
            observationFilePath,
            "Author,ObservationText,Timestamp,ID,Location\n" +
            "Oliver,Penguin,12345,1,location x\n" +
            "Oliver,Seal,12346,2,location x\n"
        );

        // Act
        var result =
            db.Read().Where(observation => observation.ID == 2).ToList();

        // Assert
        Assert.Single(result);

        Assert.Equal(
            "Seal",
            result[0].ObservationText
        );

        Assert.Equal(
            2,
            result[0].ID
        );
    }

    [Fact]
    public void GetObservationsByLocation_ReturnsOnlyMatchingObservations()
    {
        var db = CSVDatabase<Observation>.getInstance("bison_observe_cli_db.csv");

        db.Store(new Observation
        {
            Author = "Oliver",
            ObservationText = "Penguin",
            Timestamp = 1,
            ID = 1,
            Location = "Copenhagen Zoo"
        });

        db.Store(new Observation
        {
            Author = "Oliver",
            ObservationText = "Seal",
            Timestamp = 2,
            ID = 2,
            Location = "Odense Zoo"
        });

        var result =
            db.Read().Where(observation => 
                        observation.Location.Equals("Copenhagen Zoo"))
              .ToList();

        Assert.Single(result);
        Assert.Equal("Penguin", result[0].ObservationText);
    }

    [Fact]
    public void GetObservationsByLocation_ReturnsEmpty_ForUnknownLocation()
    {
        var db = CSVDatabase<Observation>.getInstance("bison_observe_cli_db.csv");

        var result =
            db.Read().Where(observation => observation.Location.Equals("Moon"))
              .ToList();

        Assert.Empty(result);
    }

    public void Dispose()
    {
        // Clean up after every test
        if (File.Exists(observationFilePath))
        {
            File.Delete(observationFilePath);
        }
    }
}