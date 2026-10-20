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
}