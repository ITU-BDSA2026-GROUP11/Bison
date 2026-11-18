using Bison.Core.Models;
using Bison.Infastructure.DTO;
using Bison.Infastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationDetailsModel : PageModel
{
    private readonly IObservationService _service;

    // Used to find the id of the logged-in user
    private readonly UserManager<Author> _userManager;

    public PostDTO? Observation { get; set; }
    public List<PostDTO> Comments { get; set; } = new();
    public List<proposalDTO> Proposals { get; set; } = new();

    // Text typed into the comment form
    [BindProperty]
    public string? CommentText { get; set; }

    // Shown under the comment form when adding a comment failed
    public string? CommentError { get; set; }

    public ObservationDetailsModel(IObservationService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    public ActionResult OnGet(int id)
    {
        // Unknown id: answer with 404 instead of an empty page
        if (!LoadObservation(id))
        {
            return NotFound();
        }

        return Page();
    }

    // Handles the comment form (asp-page-handler="Comment")
    public ActionResult OnPostComment(int id)
    {
        // Only logged-in users may comment; others are sent to the login page
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (!int.TryParse(_userManager.GetUserId(User), out int authorId))
        {
            return Challenge();
        }

        var result = _service.AddComment(id, authorId, CommentText);

        if (result.Succeeded)
        {
            // Redirect so refreshing the page does not post the comment again
            return RedirectToPage(new { id });
        }

        // Show the page again with the error message and the typed text kept
        CommentError = result.ErrorMessage;

        if (!LoadObservation(id))
        {
            return NotFound();
        }

        return Page();
    }

    // Loads the observation with its comments and proposals.
    // Returns false if no observation has that id.
    private bool LoadObservation(int id)
    {
        Observation = _service.GetObservation(id);

        if (Observation is null)
        {
            return false;
        }

        Comments = _service.GetComments(id);
        Proposals = _service.GetProposals(id);

        return true;
    }
}