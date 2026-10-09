using Bison.Razor.DTO;

namespace Bison.Razor;

// Id comes last with a default value, so existing code that creates
// ObservationViewModel with three values keeps working.
public record ObservationViewModel(string Author, string Message, string Timestamp, int Id = 0);

public record CommentViewModel(string Author, string Message, string Timestamp);

public record ProposalViewModel(string Author, string TaxonId, string Timestamp);

public interface IObservationService
{
    // Accepts a page number - default is 1
    public List<PostDTO> GetObservations(int page = 1);

    // Accepts a page number - default is 1
    public List<PostDTO> GetObservationsFromAuthor(string author, int page = 1);

    // Returns one observation, or null if no observation has that id
    public PostDTO? GetObservation(int id);

    // Returns all comments on an observation, oldest first
    public List<PostDTO> GetComments(int observationId);

    // Returns all taxon proposals on an observation, oldest first
    public List<PostDTO> GetProposals(int observationId);
}