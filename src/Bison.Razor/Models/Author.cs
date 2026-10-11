namespace Bison.Razor.Models;

public class Author
{
    public int AuthorId { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    // All posts written by this author
    public List<Post> Posts { get; set; } = new();
}