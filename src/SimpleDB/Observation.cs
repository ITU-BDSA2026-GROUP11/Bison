namespace SimpleDB;

public class Observation
{

    public string Author { get; set; } = "";
    public string ObservationText { get; set; } = "";
    public long Timestamp { get; set; }
    public int ID { get; set; }
    public string Location { get; set; } = "";
    
}