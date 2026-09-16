using SimpleDB;

namespace Bison.CLI
{
    public static class UserInterface
    {

        public static void PrintObservations(IEnumerable<Observation> observations)
        {
            foreach (var record in observations)
            {
                var observeTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).ToLocalTime();
                var timeFormatted = observeTime.ToString("dd/MM/yy HH:mm:ss");
                Console.WriteLine(
                    record.Author + " @ " + timeFormatted +
                    " [" + record.Location + "]-" + "ID[" + record.ID +
                    "]: " + record.ObservationText);
            }
        }

        public static void PrintCommentsUsingID(IEnumerable<Observation> observation, IEnumerable<Comment> comment)
        {

            foreach (var recordObservation in observation)
            {
                var observeTime = DateTimeOffset.FromUnixTimeSeconds(recordObservation.Timestamp).ToLocalTime();
                var timeFormatted = observeTime.ToString("dd'/'MM'/'yy HH:mm:ss");
                Console.WriteLine(
                    recordObservation.Author + " @ " + timeFormatted +
                    " [" + recordObservation.Location + "]-" +
                    "ID[" + recordObservation.ID + "]: " +
                    recordObservation.ObservationText);
            }

            //Reading the Cheeps from the CSVDatabase
            foreach (var recordComment in comment)
            {
                //Formatting the Cheep and printing it to console
                var observeTime = DateTimeOffset.FromUnixTimeSeconds(recordComment.Timestamp).ToLocalTime();
                var timeFormatted = observeTime.ToString("dd'/'MM'/'yy HH:mm:ss");
                Console.WriteLine(
                    "(comment) " + recordComment.Author +
                    " @ " + timeFormatted + ": " +
                    recordComment.ObservationText);
            }
        }

        //For printing Errors in console to the user
        public static void PrintError(string message)
        {
            Console.WriteLine(message);
        }


    }
}






