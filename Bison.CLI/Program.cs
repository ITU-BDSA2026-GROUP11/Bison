
using System;
using System.Globalization;
using CsvHelper;


namespace Bison.CLI
{
static class Program{

public record Cheep(string Author, string Observation, long Timestamp);
static string filePath = Path.GetFullPath("bison_observe_cli_db.csv");

static int Main(string[] args)
        {
        string line = args[0].ToLowerInvariant();
        if(line == "read")
            {
                read();
            }
        
        if(line == "observe")
            {
                string observation = "";
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
                }
                } catch(Exception e)
            {
                Console.WriteLine(e);
            }


        }

static void observe(string observation)
        {
            try
            {
                var fileExist = File.Exists(filePath);

                Cheep record = new(Environment.UserName, observation, DateTimeOffset.Now.ToUnixTimeSeconds());

                using var writer = new StreamWriter(filePath, true);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                if (!fileExist)
                {
                    csv.WriteHeader<Cheep>();
                    csv.NextRecord();
                }

                csv.WriteRecord(record);
                csv.NextRecord();
            
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}
        
