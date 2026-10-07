namespace Bison.Razor.Models;

public class Comment : Post
{
    public int ObservationId { get; set; }

    public required Observation Observation { get; set; }
}