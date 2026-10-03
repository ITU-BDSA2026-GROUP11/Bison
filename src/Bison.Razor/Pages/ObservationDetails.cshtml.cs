using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationDetailsModel : PageModel
{
    private readonly IObservationService _service;

    public ObservationViewModel? Observation { get; set; }
    public List<CommentViewModel> Comments { get; set; } = new();
    public List<ProposalViewModel> Proposals { get; set; } = new();

    public ObservationDetailsModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet(int id)
    {
        Observation = _service.GetObservation(id);

        // Unknown id: answer with 404 instead of an empty page
        if (Observation is null)
        {
            return NotFound();
        }

        Comments = _service.GetComments(id);
        Proposals = _service.GetProposals(id);

        return Page();
    }
}