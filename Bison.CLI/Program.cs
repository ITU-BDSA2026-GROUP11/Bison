using System.Globalization;
using CsvHelper.Expressions;
using SimpleDB;


namespace Bison.CLI
{
    class Program
    {
        public record Cheep(string Author, string Observation, long Timestamp);

        static void Main(string[] args)
        {
            IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();
            string line = args[0].ToLowerInvariant();

            if (line == "read")
            {
                //Reading the Cheeps from the CSVDatabase
                foreach (Cheep record in db.Read()){
                //Formatting the Cheep and printing it to console
                var observeTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).ToLocalTime();
                var timeFormatted = observeTime.ToString("dd'/'MM'/'yy HH:mm:ss");
                Console.WriteLine(record.Author + " @ " + timeFormatted + ": " + record.Observation);
                }
            }
            if (line == "observe")
            {
                //Parsing user input
                string observation = "";
                for (int i = 1; i < args.Length; i++)
                {
                    observation = observation + args[i] + " ";
                }

                //Creating the Cheep and writing it to the CSVDatabase
                Cheep record = new(Environment.UserName, observation, DateTimeOffset.Now.ToUnixTimeSeconds());
                db.Store(record);
            }
        }
    }
}

