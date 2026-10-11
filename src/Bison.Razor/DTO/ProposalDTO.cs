
namespace Bison.Razor.DTO;

public class proposalDTO
{
    public required string Text { get; set;}
    public required string Author { get; set;}
    public required string TimeStamp { get; set;}
    public int TaxonId { get; set; }
}