namespace Bison.Razor.Models;

public class Author
{
    public int AuthorId { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}