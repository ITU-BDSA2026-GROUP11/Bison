using Bison.Razor.Repositories;

namespace Bison.Razor;

public class ObservationService : IObservationService
{
    // Gives this service access to post data through the repository interface
    private readonly IPostRepository _repository;

    // Amount of observations each page holds
    private const int PageSize = 32;

    // Gets the repository through dependency injection
    public ObservationService(IPostRepository repository)
    {
        _repository = repository;
    }

    // Gets observations from all users for a specific page
    public List<ObservationViewModel> GetObservations(int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        return _repository.GetObservations(page, PageSize);
    }

    // Gets observations written by one specific user for a specific page
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        return _repository.GetObservationsFromAuthor(author, page, PageSize);
    }

    // Gets one observation and its author
    public ObservationViewModel? GetObservation(int id)
    {
        return _repository.GetObservation(id);
    }

    // Gets all comments on an observation, oldest first
    public List<CommentViewModel> GetComments(int observationId)
    {
        return _repository.GetComments(observationId);
    }

    // Gets all taxon proposals on an observation, oldest first
    public List<ProposalViewModel> GetProposals(int observationId)
    {
        return _repository.GetProposals(observationId);
    }
}