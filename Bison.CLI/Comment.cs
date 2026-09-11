
namespace Bison.CLI;

public class Comment
{
    public string Auther {get; set;}
    public string CommentText {get; set;}
    public long Timestamp {get; set;}
    public int ID {get; set}

    public Comment(string Auther, string CommentText, long Timestamp, int ID)
    {
        this.Auther = Auther;
        this.CommentText = CommentText;
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
        Cheep record = new(Auther, CommentText, Timestamp, ID);
        return record;
    }
}