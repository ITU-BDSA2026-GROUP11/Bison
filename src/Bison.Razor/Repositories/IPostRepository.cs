namespace Bison.Razor.Repositories;

// Defines all database queries related to posts (observations, comments, proposals).
// Other classes depend on this interface, never on PostRepository directly.
public interface IPostRepository
{
    // Returns one page of observations from all users, newest first
    List<ObservationViewModel> GetObservations(int page, int pageSize);

    // Returns one page of observations from one user, newest first
    List<ObservationViewModel> GetObservationsFromAuthor(string author, int page, int pageSize);

    // Returns one observation, or null if no observation has that id
    ObservationViewModel? GetObservation(int id);

    // Returns all comments on an observation, oldest first
    List<CommentViewModel> GetComments(int observationId);

    // Returns all taxon proposals on an observation, oldest first
    List<ProposalViewModel> GetProposals(int observationId);
}