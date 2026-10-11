namespace Bison.Razor.Models;

public class Observation : Post
{
    // Foreign key for the taxon
    public int TaxonId { get; set; }

    // Taxon associated with the observation
    public Taxon Taxon { get; set; } = null!;

    // Comments on this observation
    public List<Comment> Comments { get; set; } = new();

    // Taxon proposals for this observation
    public List<Proposal> Proposals { get; set; } = new();
}