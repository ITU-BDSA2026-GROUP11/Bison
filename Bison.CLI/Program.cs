using System;
using SimpleDB;
using DocoptNet;//this is the CLI console parser (parses user input)


namespace Bison.CLI
{
    class Program
    {
        private const string Usage = @"
            Usage:
                Bison read
                Bison observe <observation>
                Bison (-h | --help)

            Options:
                -h --help    Show this help message.
        ";


        static void Main(string[] args)
        {
            IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();

            var arguments = new Docopt().Apply(Usage, args, help: true);

            if (arguments!["read"].IsTrue)// the "!" supress the error arguments may be null
            {
                //UserInterface handles writing to the console
                UserInterface.PrintObservations(db.Read());
            }
            if (arguments["observe"].IsTrue)
            {
                //Parses user input from <observation> to string
                string observation = arguments["<observation>"].ToString();

                //Creating the Cheep and writing it to the CSVDatabase
                Cheep record = new(Environment.UserName, observation, DateTimeOffset.Now.ToUnixTimeSeconds());
                db.Store(record);
            }
        }
    }
}

