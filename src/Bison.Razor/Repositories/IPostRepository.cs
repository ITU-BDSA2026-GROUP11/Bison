using Bison.Razor.DTO;

namespace Bison.Razor.Repositories;

// Defines all database queries related to posts (observations, comments, proposals).
// Other classes depend on this interface, never on PostRepository directly.
public interface IPostRepository
{
    // Returns one page of observations from all users, newest first
    List<PostDTO> GetObservations(int page, int pageSize);

    // Returns one page of observations from one user, newest first
    List<PostDTO> GetObservationsFromAuthor(string author, int page, int pageSize);

    // Returns one observation, or null if no observation has that id
    PostDTO? GetObservation(int id);

    // Returns all comments on an observation, oldest first
    List<PostDTO> GetComments(int observationId);

    // Returns all taxon proposals on an observation, oldest first
    List<PostDTO> GetProposals(int observationId);
}