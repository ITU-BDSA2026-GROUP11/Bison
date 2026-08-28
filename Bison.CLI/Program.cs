
using System;


namespace Bison.CLI
{
static class program{
static string Auther, Observation;
static string filePath = Path.GetFullPath("bison_observe_cli_db.csv");

//string filePath = @"C:\Users\Bruger\Desktop\ITU\ToBeRenamed\Project\Bison\Bison.CLI\bison_observe_cli_db.csv";
static int Main(string[] args)
        {
        string line = args[0];
        if(line == "read")
            {
                read();
            }
    return 0;
    }

static void read()
        {
            
            try{


            string Auther, Observation;
            string filePath = Path.GetFullPath("bison_observe_cli_db.csv");

            //string filePath = @"C:\Users\Bruger\Desktop\ITU\ToBeRenamed\Project\Bison\Bison.CLI\bison_observe_cli_db.csv";
            using(StreamReader reader = new StreamReader(File.OpenRead(filePath)))
                {
                    string line;
                    string waste = reader.ReadLine();
                    while(!reader.EndOfStream)
                        {
                            line = reader.ReadLine();

                            string[] Observe = line.Split(","); 
                            Auther = Observe[0];
                            Observation = Observe[1];
                            var observeTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(Observe[2])).ToLocalTime();
                            var timeFormatted = observeTime.ToString("dd'/'MM'/'yy HH:mm:ss");
                            Console.WriteLine(Auther + " @ " + timeFormatted + ": " + Observation);

                        }
                }
                } catch(Exception e)
            {
                Console.WriteLine("EEEEEEEEEEEEEEEE");
            }


        }
    }
}
        
