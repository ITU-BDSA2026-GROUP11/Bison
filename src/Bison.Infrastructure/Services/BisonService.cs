using Bison.Infastructure.DTO;
using Bison.Core.Models;

namespace Bison.Infastructure;

// Id comes last with a default value, so existing code that creates
// ObservationViewModel with three values keeps working.
public record ObservationViewModel(string Author, string Message, string Timestamp, int Id = 0);

public record CommentViewModel(string Author, string Message, string Timestamp);

public record ProposalViewModel(string Author, string TaxonId, string Timestamp);

// Result of trying to add a comment or proposal.
// Succeeded is false when validation or saving failed, and ErrorMessage says why.
public record PostResult(bool Succeeded, string? ErrorMessage = null)
{
    public static PostResult Ok() => new(true);
    public static PostResult Fail(string message) => new(false, message);
}

public interface IObservationService
{
    // Accepts a page number - default is 1
    public List<PostDTO> GetObservations(int page = 1);

    // Accepts a page number - default is 1
    public List<PostDTO> GetObservationsFromAuthor(string author, int page = 1);

    // Returns one page of observations whose taxon is the given taxon or a subtaxon of it.
    // Returns an empty list if no taxon has that id.
    public List<PostDTO> GetObservationsByTaxon(int taxonId, int page = 1);

    // Returns one taxon, or null if no taxon has that id
    public Taxon? GetTaxon(int taxonId);

    // Returns all taxa (id and display name), sorted by name
    public List<TaxonDTO> GetAllTaxa();

    // Returns one observation, or null if no observation has that id
    public PostDTO? GetObservation(int id);

    // Returns all comments on an observation, oldest first
    public List<PostDTO> GetComments(int observationId);

    // Returns all taxon proposals on an observation, oldest first
    public List<proposalDTO> GetProposals(int observationId);

    public Task addObservation(string text, int authorID, int taxonId, string imgPath);
    // Adds a comment to an observation.
    // Fails if the text is empty, the observation does not exist, or saving fails.
    public PostResult AddComment(int observationId, int authorId, string? text);

    // Adds a taxon proposal to an observation. The text is an optional reason.
    // Fails if the observation or taxon does not exist, or saving fails.
    public PostResult AddProposal(int observationId, int authorId, int taxonId, string? text);
}