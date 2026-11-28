using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bison.Core.Models;

public class Observation : Post
{
    // Foreign key for the taxon
    public int TaxonId { get; set; }

    // Taxon associated with the observation
    public Taxon Taxon { get; set; } = null!;

    public Taxon getTaxon() => Taxon;

    // Comments on this observation
    public List<Comment> Comments { get; set; } = new();

    // Taxon proposals for this observation
    public List<Proposal> Proposals { get; set; } = new();

    public string? ImgPath {get; set;}

    // Optional place where the animal was observed, for example "Amager Fælled".
    // Null when the user did not give a location.
    [MaxLength(100)]
    public string? Location { get; set; }
}