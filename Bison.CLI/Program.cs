
using System;
using System.Globalization;
using CsvHelper;


namespace Bison.CLI
{
static class program{

public record Cheep(string Author, string Observation, long Timestamp);

static string Auther, Observation;
static string filePath = Path.GetFullPath("bison_observe_cli_db.csv");

//string filePath = @"C:\Users\Bruger\Desktop\ITU\ToBeRenamed\Project\Bison\Bison.CLI\bison_observe_cli_db.csv";
static int Main(string[] args)
        {
        string line = args[0].ToLowerInvariant();
        if(line == "read")
            {
                read();
            }
        
        if(line == "observe")
            {
                String observation = "";
                for(int i = 1; i < args.Length; i++)
                {
                    observation = observation + args[i] + " ";
                }
                observe(observation);
            }
    return 0;
    }

static void read()
        {
            
            try{


            string Auther, Observation;
            string filePath = Path.GetFullPath("bison_observe_cli_db.csv");

            //string filePath = @"C:\Users\Bruger\Desktop\ITU\ToBeRenamed\Project\Bison\Bison.CLI\bison_observe_cli_db.csv";
            using(var reader = new StreamReader(File.OpenRead(filePath)))
            using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Cheep>();
                    
                    foreach (var record in records)
                    {
                        var observeTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).ToLocalTime();
                        var timeFormatted = observeTime.ToString("dd'/'MM'/'yy HH:mm:ss");
                        Console.WriteLine(record.Author + " @ " + timeFormatted + ": " + record.Observation);
                    }

                    Console.WriteLine();
                }
                } catch(Exception e)
            {
                Console.WriteLine(e);
            }


        }

static void observe(String observation)
        {
            try
            {
                string filePath = Path.GetFullPath("bison_observe_cli_db.csv");
            var currentTime = DateTimeOffset.Now;
            //File.AppendAllText(filePath, Environment.UserName + ", " + observation + ", " + currentTime.ToUnixTimeSeconds());
            using(StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(Environment.UserName + "," + '"'+ observation + '"' + "," + currentTime.ToUnixTimeSeconds());
            }
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}
        
