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
            IDatabaseRepository<Cheep> dbCheep = new CSVDatabase<Cheep>();
            dbCheep.setFilePath("observation");
            IDatabaseRepository<Comment> dbComment = new CSVDatabase<Comment>();
            dbComment.setFilePath("comment");
            var arguments = new Docopt().Apply(Usage, args, help: true);

            if (arguments!["read"].IsTrue)// the "!" supress the error: arguments may be null
            {
                //UserInterface handles writing to the console
                UserInterface.PrintObservations(dbCheep.Read());
            }
            if (arguments["observe"].IsTrue)
            {   
                //Parses user input from <observation> to string
                string observation = arguments["<observation>"].ToString();

                //Get previous ID
                var records = dbCheep.Read();
                int nextID = records.Max(c => c.ID) + 1;
                //Creating the Cheep and writing it to the CSVDatabase
                Cheep record = new(Environment.UserName, observation, DateTimeOffset.Now.ToUnixTimeSeconds(), nextID);
                dbCheep.Store(record);
            }
            if (arguments["comment"].IsTrue)
            {
                //Checking for observation ID
                string IDString = arguments["<id>"].ToString();

                if (dbCheep.doesIdExist(IDString))
                {
                var records = dbComment.Read();
                int nextID = records.Max(c => c.ID) + 1;

                    string commentText = arguments["<comment>"].ToString();
                    Comment comment = new(){
                        Author = Environment.UserName, 
                        ObservationText = commentText, 
                        Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(), 
                        ID = nextID,
                        ObservationID = Convert.ToInt32(IDString)};
                    dbComment.Store(comment);
                }
                else
                {
                    Console.WriteLine("Sorry, this ID does not exist");
                }
                
            }
            if (arguments["discussion"].IsTrue)
            {
                //Checking for observation ID
                string IDString = arguments["<id>"].ToString();
                //get the specific observation with this ID
                IEnumerable<Observation> observation = dbCheep.getObservationUsingId(Convert.ToInt32(IDString));
                //get the specific comment that reference this ID
                IEnumerable<Comment> comments = dbComment.getCommentsUsingId(Convert.ToInt32(IDString));

                UserInterface.PrintCommentsUsingID(observation, comments);
            }
        }

    }
}

