using Bison.CLI;

namespace SimpleDB.Tests;

public class SimpleDB_UnitTest
{
    [Fact]
    public void DoesIdExist_ReturnsFalse_ForNonExistingId()
    {
        // Arrange
        var db = new CSVDatabase<Cheep>();
        db.setFilePath("test");

        File.WriteAllText(
        Path.GetFullPath("bison_test.csv"),
        "Author,ObservationText,Timestamp,ID\n" +
        "Oliver,Penguin,12345,1\n"
        );

        // Act
        bool result = db.doesIdExist("999");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesIdExist_ReturnsTrue_ForExistingId()
    {
        var db = new CSVDatabase<Cheep>();
        db.setFilePath("test");

        File.WriteAllText(
            Path.GetFullPath("bison_test.csv"),
            "Author,ObservationText,Timestamp,ID\n" +
            "Oliver,Penguin,12345,1\n"
        );

        bool result = db.doesIdExist("1");

        Assert.True(result);
    }


    //Testing Store and Retrieve methods
    [Fact]
    public void Store_Then_Read_ReturnsStoredObservation()
    {
        var db = new CSVDatabase<Cheep>();
        db.setFilePath("test");

        File.Delete(Path.GetFullPath("bison_test.csv"));

        var cheep = new Cheep(
            "Oliver",
            "Penguin",
            12345,
            1
        );

        db.Store(cheep);

        var result = db.Read().ToList();

        Assert.Single(result);
        Assert.Equal("Penguin", result[0].ObservationText);
    }

    [Fact]
    public void GetObservationUsingId_ReturnsCorrectObservation()
    {
        var db = new CSVDatabase<Cheep>();
        db.setFilePath("test");

        File.WriteAllText(
            Path.GetFullPath("bison_test.csv"),
            "Author,ObservationText,Timestamp,ID\n" +
            "Oliver,Penguin,12345,1\n" +
            "Oliver,Seal,12346,2\n"
        );

        var result = db.getObservationUsingId(2);

        Assert.Single(result);

        Assert.Equal(
            "Seal",
            result.First().ObservationText
        );
    }


}