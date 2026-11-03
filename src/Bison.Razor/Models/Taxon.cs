using System.ComponentModel.DataAnnotations.Schema;
namespace Bison.Razor.Models;

public class Taxon
{
    public int TaxonId { get; set; }

    public string? dwc_TaxonID { get; set; } = "";

    public string? VernacularName { get; set; }

    public int? ParentId { get; set; }

    public Taxon? Parent { get; set; }

    public ICollection<Taxon> Children { get; set; } = new List<Taxon>();

    // True if this taxon is the ancestor itself, or lies anywhere below it.
    // Walks up the Parent chain, so the Parent chain must be loaded.
    public bool IsSubTaxon(Taxon ancestor)
    {
        for (Taxon? t = this; t != null; t = t.Parent)
        {
            if (t.TaxonId == ancestor.TaxonId)
            {
                return true;
            }
        }
        return false;
    }
}