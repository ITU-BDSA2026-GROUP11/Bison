namespace Bison.Razor.Models;

public class Post
{
    public int PostId { get; set; }

    public string Text { get; set; } = "";

    public DateTime TimeStamp { get; set; }

    // Foreign key for the author
    public int AuthorId { get; set; }

    // Author who created this post
    public Author Author { get; set; } = null!;
}