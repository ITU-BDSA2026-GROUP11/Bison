using SimpleDB;

namespace Bison.CLI.Tests;

public class Bison_UnitTest
{
    [Fact]
    public void CommentWithNonExistingObservationIdIsRejected()
    {
        var db = new CSVDatabase<Cheep>();

        db.setFilePath("test");

        File.WriteAllText("bison_test.csv", "");

        bool exists = db.doesIdExist("99999");

        Assert.False(exists);
    }

    [Fact]
    public void NextIdIsOneGreaterThanHighestExistingId()
    {
        var records = new List<Cheep>
    {
        new("A","Obs1",0,1),
        new("B","Obs2",0,2),
        new("C","Obs3",0,3)
    };

        int nextId = records.Max(c => c.ID) + 1;

        Assert.Equal(4, nextId);
    }

    [Fact]
    public void UnixTimestampCanBeConvertedToDate()
    {
        long timestamp = 1788959685;

        var date =
            DateTimeOffset
                .FromUnixTimeSeconds(timestamp);

        Assert.Equal(2026, date.Year);
    }


}