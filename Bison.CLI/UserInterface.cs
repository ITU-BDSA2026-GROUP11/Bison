using SimpleDB;


namespace Bison.CLI
{
    public static class UserInterface
    {

        public static void PrintObservations(IEnumerable<Cheep> observation)
        {

            //Reading the Cheeps from the CSVDatabase
            foreach (var record in observation)
            {
                //Formatting the Cheep and printing it to console
                var observeTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).ToLocalTime();
                var timeFormatted = observeTime.ToString("dd'/'MM'/'yy HH:mm:ss");
                Console.WriteLine(record.Author + " @ " + timeFormatted + ": " + record.Observation);
            }
        }
    }
}






