namespace Bison.Razor.Models;

public class Proposal : Post
{
    public int ObservationId { get; set; }

    public required Observation Observation { get; set; }

    public int TaxonId { get; set; }

    public required Taxon Taxon { get; set; }
}