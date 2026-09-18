using DocoptNet;
using SimpleDB;

namespace Bison.CLI
{
    class Program
    {
        private const string Usage = @"
            Usage:
                Bison read
                Bison observe <observation> <location>
                Bison location <location>
                Bison comment <id> <comment>
                Bison discussion <id>
                Bison (-h | --help)

            Options:
                -h --help    Show this help message.
        ";

        static void Main(string[] args)
        {
            // Get the single CSVDatabase instance
            var obs_db = CSVDatabase<Observation>.getInstance("bison_observe_cli_db.csv");
            var evil_db = CSVDatabase<Comment>.getInstance("bison_comments_cli_db.csv");

            // Parse command line arguments with DocoptNet
            var arguments = new Docopt().Apply(
                Usage,
                args,
                help: true
            );

            // READ------------------------------------------------

            if (arguments!["read"].IsTrue)
            {
                var observations = obs_db.Read();

                UserInterface.PrintObservations(observations);

                return;
            }

            // OBSERVE------------------------------------------------

            if (arguments["observe"].IsTrue)
            {
                string observationText =
                    arguments["<observation>"].ToString();

                string location =
                    arguments["<location>"].ToString();

                var observations =
                    obs_db.Read();

                int nextID = observations.Any()
                    ? observations.Max(o => o.ID) + 1
                    : 1;

                Observation observation = new()
                {
                    Author = Environment.UserName,
                    ObservationText = observationText,
                    Location = location,
                    Timestamp =
                        DateTimeOffset.Now.ToUnixTimeSeconds(),
                    ID = nextID
                };

                obs_db.Store(observation);

                return;
            }
            
            // LOCATION------------------------------------------------

            if (arguments["location"].IsTrue)
            {
                string location =
                    arguments["<location>"].ToString();

                //var observations =
                //    db.GetObservationsByLocation(location);
                var observations = obs_db.Read().Where(observation => 
                        observation.Location.Equals(location, StringComparison.OrdinalIgnoreCase));

                UserInterface.PrintObservations(observations);

                return;
            }

            // COMMENT------------------------------------------------


            if (arguments["comment"].IsTrue)
            {
                string idText =
                    arguments["<id>"].ToString();

                // Validate that the ID is actually a number
                if (!int.TryParse(idText, out int observationId))
                {
                    UserInterface.PrintError(
                        "The observation ID must be a number."
                    );

                    return;
                }

                // Check that the observation exists
                int checkID = int.Parse(idText);
                if (!obs_db.Read().Any(observation => observation.ID == checkID))
                {
                    UserInterface.PrintError(
                        "Sorry, this observation ID does not exist."
                    );

                    return;
                }

                string commentText =
                    arguments["<comment>"].ToString();

                var comments =
                    evil_db.Read();

                int nextID = comments.Any()
                    ? comments.Max(c => c.ID) + 1
                    : 1;

                Comment comment = new()
                {
                    Author = Environment.UserName,
                    ObservationText = commentText,
                    Timestamp =
                        DateTimeOffset.Now.ToUnixTimeSeconds(),
                    ID = nextID,
                    ObservationID = observationId
                };

                evil_db.Store(comment);

                return;
            }


            // DISCUSSION------------------------------------------------

            if (arguments["discussion"].IsTrue)
            {
                string idText =
                    arguments["<id>"].ToString();

                // Validate ID
                if (!int.TryParse(idText, out int observationId))
                {
                    UserInterface.PrintError(
                        "The observation ID must be a number."
                    );

                    return;
                }

                // Check whether observation exists
                int checkID = int.Parse(idText);
                if (!obs_db.Read().Any(observation => observation.ID == checkID))
                {
                    UserInterface.PrintError(
                        "Sorry, this observation ID does not exist."
                    );

                    return;
                }

                var observation =
                    obs_db.Read().Where(observation => observation.ID == checkID);

                var comments =
                    evil_db.Read().Where(comment => comment.ID == checkID);

                UserInterface.PrintCommentsUsingID(
                    observation,
                    comments
                );
            }
        }
    }
}