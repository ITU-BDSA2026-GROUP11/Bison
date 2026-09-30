using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;

    public List<ObservationViewModel> Observations { get; set; } = new();
    public int CurrentPage { get; set; } = 1;

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int? page)
    {
        CurrentPage = page ?? 1;

        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        Observations = _service.GetObservationsFromAuthor(author, CurrentPage);

        return Page();
    }
}