using System.Net.NetworkInformation;
using SimpleDB;


namespace Bison.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();
            
            string line = args[0].ToLowerInvariant();

            if (line == "read")
            {
                //UserInterface handles writing to the console
                UserInterface.PrintObservations(db.Read());
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

