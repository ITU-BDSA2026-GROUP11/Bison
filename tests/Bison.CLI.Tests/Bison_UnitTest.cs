using SimpleDB;

namespace Bison.CLI.Tests;

public class Bison_UnitTest
{
    [Fact]
    public void NonExistingObservationIdReturnsFalse()
    {
        var db = CSVDatabase<Observation>.getInstance("bison_observe_cli_db.csv");

        bool exists = db.Read().Any(observation => observation.ID == 99999);

        Assert.False(exists);
    }

    [Fact]
    public void NextIdIsOneGreaterThanHighestExistingId()
    {
        var records = new List<Observation>
        {
            new()
            {
                Author = "A",
                ObservationText = "Obs1",
                Timestamp = 0,
                ID = 1
            },
            new()
            {
                Author = "B",
                ObservationText = "Obs2",
                Timestamp = 0,
                ID = 2
            },
            new()
            {
                Author = "C",
                ObservationText = "Obs3",
                Timestamp = 0,
                ID = 3
            }
        };

        int nextId = records.Max(o => o.ID) + 1;

        Assert.Equal(4, nextId);
    }

    [Fact]
    public void UnixTimestampCanBeConvertedToDate()
    {
        long timestamp = 1788959685;

        var date =
            DateTimeOffset.FromUnixTimeSeconds(timestamp);

        Assert.Equal(2026, date.Year);
    }
}