namespace Bison.Razor.Models;

public class Taxon
{
    public int TaxonId { get; set; }

    public required string DwcTaxonId { get; set; }//This need to be string or int depending on the data unsure of which

    public required string DanishVernacularName { get; set; }

    public int? ParentId { get; set; }

    public Taxon? Parent { get; set; }

    public ICollection<Taxon> Children { get; set; } = new List<Taxon>();
}