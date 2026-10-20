namespace Bison.Razor.Models;

public class Comment : Post
{
    // Foreign key for the observation
    public int ObservationId { get; set; }

    // Observation this comment belongs to
    public Observation Observation { get; set; } = null!;
}