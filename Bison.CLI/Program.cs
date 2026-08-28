
try{
string Auther, Observation, timestamp;
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
                timestamp = Observe[2];

                Console.WriteLine(Auther + " & " + Observation + " & " + timestamp);

            }
    }
    } catch(Exception e)
{
    Console.WriteLine("EEEEEEEEEEEEEEEE");
}