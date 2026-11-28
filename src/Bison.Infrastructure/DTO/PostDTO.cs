
namespace Bison.Infastructure.DTO;

public class PostDTO
{
    
    public required string Text { get; set;}
    public required string Author { get; set;}
    public required string TimeStamp { get; set;}

    public int PostId { get; set; }

    public string? ImgPath {get; set; }

    // Where the animal was observed, or null if no location was given
    public string? Location { get; set; }
}