using System;
using SimpleDB;
using DocoptNet;//this is the CLI console parser (parses user input)


namespace Bison.CLI
{
    class Program
    {
        //This string is the contains info for DocoptNet and shows available commmands
        private const string Usage = @"
            Usage:
                Bison read
                Bison observe <observation>
                Bison comment <id> <comment>
                Bison discussion <id>
                Bison (-h | --help)

            Options:
                -h --help    Show this help message.
        ";

        static void Main(string[] args)
        {
            IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();
            var arguments = new Docopt().Apply(Usage, args, help: true);

            if (arguments!["read"].IsTrue)// the "!" supress the error: arguments may be null
            {
                db.setFilePath("observation");
                //UserInterface handles writing to the console
                UserInterface.PrintObservations(db.Read());
            }
            if (arguments["observe"].IsTrue)
            {   
                db.setFilePath("observation");
                //Parses user input from <observation> to string
                string observation = arguments["<observation>"].ToString();

                //Get previous ID
                var records = db.Read();
                int nextID = records.Max(c => c.ID) + 1;
                //Creating the Cheep and writing it to the CSVDatabase
                Cheep record = new(Environment.UserName, observation, DateTimeOffset.Now.ToUnixTimeSeconds(), nextID);
                db.Store(record);
            }
            if (arguments["comment"].IsTrue)
            {
                db.setFilePath("observation");
                //Checking for observation ID
                string IDString = arguments["<id>"].ToString();

                if (db.doesIdExist(IDString))
                {
                    db.setFilePath("comment");
                    string comment = arguments["<comment>"].ToString();
                    Cheep record = new(Environment.UserName, comment, DateTimeOffset.Now.ToUnixTimeSeconds(), Convert.ToInt32(IDString));
                    db.Store(record);
                }
                else
                {
                    Console.WriteLine("Sorry, this ID does not exist");
                }
                
            }
            if (arguments["discussion"].IsTrue)
            {
                db.setFilePath("observation");
                //Checking for observation ID
                string IDString = arguments["<id>"].ToString();
                //get the specific observation with this ID

                db.setFilePath("comment");
                //UserInterface.PrintObservations(db.getComments(IDString));

            }
        }
    }
}

