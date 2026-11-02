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

    public bool IsSubTaxon(Taxon ancestor)
    {
        if(this.Parent != null)
        {
            for (Taxon t = this; t.Parent != null; t = t.Parent)
            {
                if(t.TaxonId == ancestor.TaxonId)
                {
                    return true;
                }
            } 
        }
        return false;
        
    }
}