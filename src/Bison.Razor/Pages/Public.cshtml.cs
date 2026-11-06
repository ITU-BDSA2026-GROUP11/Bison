using Bison.Infastructure.DTO;
using Bison.Infastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;

    public List<PostDTO> Observations { get; set; } = new();
    public int CurrentPage { get; set; }

    // All taxa for the taxon selector
    public List<TaxonDTO> Taxa { get; set; } = new();

    // The taxon currently filtered by, or null when showing all observations
    public int? SelectedTaxonId { get; set; }

    // Display name of the selected taxon, or null when showing all observations
    public string? SelectedTaxonName { get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

    // Gets the page number and optional taxon from the URL, for example ?page=2&taxon=5
    public ActionResult OnGet([FromQuery] int? page, [FromQuery] int? taxon)
    {
        // If no page is given, use page 1
        CurrentPage = page ?? 1;

        // Prevent page 0 or negative page numbers
        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        Taxa = _service.GetAllTaxa();
        SelectedTaxonId = taxon;

        if (taxon.HasValue)
        {
            // Filter by the taxon and its subtaxa, using the verified filter
            Observations = _service.GetObservationsByTaxon(taxon.Value, CurrentPage);
            SelectedTaxonName = Taxa.FirstOrDefault(t => t.TaxonId == taxon.Value)?.Name;
        }
        else
        {
            // Gets only the observations for the requested page
            Observations = _service.GetObservations(CurrentPage);
        }

        return Page();
    }
}