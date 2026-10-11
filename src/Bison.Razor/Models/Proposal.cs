namespace Bison.Razor.Models;

public class Proposal : Post
{
    // Foreign key for the observation
    public int ObservationId { get; set; }

    // Observation this proposal belongs to
    public Observation Observation { get; set; } = null!;

    // Foreign key for the proposed taxon
    public int TaxonId { get; set; }

    // Proposed taxon
    public Taxon Taxon { get; set; } = null!;
}