using Bison.Infastructure.DTO;
using Bison.Core.Models;

namespace Bison.Infastructure.Repositories;

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
    List<proposalDTO> GetProposals(int observationId);

    // Returns all observations, newest first, with Author and Taxon loaded.
    // Every taxon's full Parent chain is loaded, so Taxon.IsSubTaxon works.
    List<Observation> GetAllObservationsWithTaxa();

    // Returns one taxon, or null if no taxon has that id
    Taxon? GetTaxon(int taxonId);

    // Returns all taxa (id and display name), sorted by name
    List<TaxonDTO> GetAllTaxa();
}