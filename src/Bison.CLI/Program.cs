using DocoptNet;
using SimpleDB;

namespace Bison.CLI
{
    class Program
    {
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
            // Get the single CSVDatabase instance
            var db = CSVDatabase.Instance;

            // Parse command line arguments with DocoptNet
            var arguments = new Docopt().Apply(
                Usage,
                args,
                help: true
            );

            // READ------------------------------------------------

            if (arguments!["read"].IsTrue)
            {
                var observations = db.ReadObservations();

                UserInterface.PrintObservations(observations);

                return;
            }

            // OBSERVE------------------------------------------------

            if (arguments["observe"].IsTrue)
            {
                string observationText =
                    arguments["<observation>"].ToString();

                var observations =
                    db.ReadObservations();

                int nextID = observations.Any()
                    ? observations.Max(o => o.ID) + 1
                    : 1;

                Observation observation = new()
                {
                    Author = Environment.UserName,
                    ObservationText = observationText,
                    Timestamp =
                        DateTimeOffset.Now.ToUnixTimeSeconds(),
                    ID = nextID
                };

                db.StoreObservation(observation);

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
                if (!db.DoesObservationIdExist(observationId))
                {
                    UserInterface.PrintError(
                        "Sorry, this observation ID does not exist."
                    );

                    return;
                }

                string commentText =
                    arguments["<comment>"].ToString();

                var comments =
                    db.ReadComments();

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

                db.StoreComment(comment);

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
                if (!db.DoesObservationIdExist(observationId))
                {
                    UserInterface.PrintError(
                        "Sorry, this observation ID does not exist."
                    );

                    return;
                }

                var observation =
                    db.GetObservationUsingId(observationId);

                var comments =
                    db.GetCommentsUsingId(observationId);

                UserInterface.PrintCommentsUsingID(
                    observation,
                    comments
                );
            }
        }
    }
}