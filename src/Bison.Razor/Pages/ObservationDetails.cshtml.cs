using Bison.Core.Models;
using Bison.Infastructure.DTO;
using Bison.Infastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bison.Razor.Pages;

public class ObservationDetailsModel : PageModel
{
    private readonly IObservationService _service;

    // Used to find the id of the logged-in user
    private readonly UserManager<Author> _userManager;

    public PostDTO? Observation { get; set; }
    public List<PostDTO> Comments { get; set; } = new();
    public List<proposalDTO> Proposals { get; set; } = new();

    // Taxon names by id, so proposals can show a name instead of a number
    public Dictionary<int, string> TaxonNames { get; set; } = new();

    // Options for the taxon dropdown in the proposal form
    public SelectList TaxonOptions { get; set; } = null!;

    // Text typed into the comment form
    [BindProperty]
    public string? CommentText { get; set; }

    // Shown under the comment form when adding a comment failed
    public string? CommentError { get; set; }

    // Taxon chosen in the proposal form (null when nothing is chosen)
    [BindProperty]
    public int? ProposalTaxonId { get; set; }

    // Optional reason typed into the proposal form
    [BindProperty]
    public string? ProposalText { get; set; }

    // Shown under the proposal form when adding a proposal failed
    public string? ProposalError { get; set; }

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
        if (!TryGetAuthorId(out int authorId))
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

    // Handles the proposal form (asp-page-handler="Proposal")
    public ActionResult OnPostProposal(int id)
    {
        // Only logged-in users may propose; others are sent to the login page
        if (!TryGetAuthorId(out int authorId))
        {
            return Challenge();
        }

        // No taxon chosen becomes id 0, which the service rejects as invalid
        var result = _service.AddProposal(id, authorId, ProposalTaxonId ?? 0, ProposalText);

        if (result.Succeeded)
        {
            // Redirect so refreshing the page does not post the proposal again
            return RedirectToPage(new { id });
        }

        // Show the page again with the error message and the choices kept
        ProposalError = result.ErrorMessage;

        if (!LoadObservation(id))
        {
            return NotFound();
        }

        return Page();
    }

    // Finds the id of the logged-in user.
    // Returns false if nobody is logged in.
    private bool TryGetAuthorId(out int authorId)
    {
        authorId = 0;

        if (User.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        return int.TryParse(_userManager.GetUserId(User), out authorId);
    }

    // Loads the observation with its comments, proposals and the taxa.
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

        var taxa = _service.GetAllTaxa();
        TaxonNames = taxa.ToDictionary(t => t.TaxonId, t => t.Name);
        TaxonOptions = new SelectList(taxa, nameof(TaxonDTO.TaxonId), nameof(TaxonDTO.Name));

        return true;
    }
}