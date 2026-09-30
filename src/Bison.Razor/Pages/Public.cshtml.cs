using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;

    public List<ObservationViewModel> Observations { get; set; } = new();
    public int CurrentPage { get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

    // Gets the page number from the URL, for example ?page=2
    public ActionResult OnGet([FromQuery] int? page)
    {
        // If no page is given, use page 1
        CurrentPage = page ?? 1;

        // Prevent page 0 or negative page numbers
        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        // Gets only the observations for the requested page
        Observations = _service.GetObservations(CurrentPage);

        return Page();
    }
}