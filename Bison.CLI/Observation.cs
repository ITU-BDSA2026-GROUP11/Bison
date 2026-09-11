namespace Bison.CLI;
public class Observation
{

    public string Auther {get; set;}
    public string ObservationText {get; set;}
    public long Timestamp {get; set;}
    public int ID {get; set;}

    public Observation(string Auther, string ObservationText, long Timestamp, int ID)
    {
        this.Auther = Auther;
        this.ObservationText = ObservationText;
        this.Timestamp = Timestamp;
        this.ID = ID;
    }
    public int getID()
    {
        return ID;
    }
    public string getAuther()
    {
        return Auther;
    }
    public long getTimeStamp()
    {
        return Timestamp;
    }

    public Cheep makeCheep()
    {
        Cheep record = new(Auther, ObservationText, Timestamp, ID);
        return record;
    }

}