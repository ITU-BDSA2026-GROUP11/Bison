using Bison.Infastructure.DTO;
using Bison.Core.Models;
using Bison.Infastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bison.Infastructure;

public class ObservationService : IObservationService
{
    // Gives this service access to post data through the repository interface
    private readonly IPostRepository _repository;

    // The Dafny-verified taxon filter
    private readonly ITaxonFilterService _taxonFilter;

    // Amount of observations each page holds
    private const int PageSize = 32;

    // Gets the repository and the filter service through dependency injection
    public ObservationService(IPostRepository repository, ITaxonFilterService taxonFilter)
    {
        _repository = repository;
        _taxonFilter = taxonFilter;
    }

    // Gets observations from all users for a specific page
    public List<PostDTO> GetObservations(int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        return _repository.GetObservations(page, PageSize);
    }

    // Gets observations written by one specific user for a specific page
    public List<PostDTO> GetObservationsFromAuthor(string author, int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        return _repository.GetObservationsFromAuthor(author, page, PageSize);
    }

    // Gets one page of observations for a taxon and all its subtaxa
    public List<PostDTO> GetObservationsByTaxon(int taxonId, int page = 1)
    {
        // Makes sure the page number cannot be below 1
        if (page < 1)
        {
            page = 1;
        }

        // Loads all observations, with every taxon's parent chain
        var observations = _repository.GetAllObservationsWithTaxa();

        var root = _repository.GetTaxon(taxonId);
        if (root == null)
        {
            return new List<PostDTO>();
        }

        // Filter first (verified code), then page, so every page is full
        var filtered = _taxonFilter.FilterByTaxon(root, observations);

        return filtered
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(o => new PostDTO
            {
                PostId = o.PostId,
                Author = o.Author.Name,
                Text = o.Text,
                TimeStamp = o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            })
            .ToList();
    }

    // Gets one taxon
    public Taxon? GetTaxon(int taxonId)
    {
        return _repository.GetTaxon(taxonId);
    }

    // Gets all taxa for the taxon selector
    public List<TaxonDTO> GetAllTaxa()
    {
        return _repository.GetAllTaxa();
    }

    // Gets one observation and its author
    public PostDTO? GetObservation(int id)
    {
        return _repository.GetObservation(id);
    }

    // Gets all comments on an observation, oldest first
    public List<PostDTO> GetComments(int observationId)
    {
        return _repository.GetComments(observationId);
    }

    // Gets all taxon proposals on an observation, oldest first
    public List<proposalDTO> GetProposals(int observationId)
    {
        return _repository.GetProposals(observationId);
    }

    public async Task addObservation(string text, int authorID, int taxonId)
    {
        await _repository.addObservation(text, authorID, taxonId);
    // Validates and stores a new comment
    public PostResult AddComment(int observationId, int authorId, string? text)
    {
        // A comment must contain text
        if (string.IsNullOrWhiteSpace(text))
        {
            return PostResult.Fail("A comment cannot be empty.");
        }

        // The observation must exist
        if (_repository.GetObservation(observationId) == null)
        {
            return PostResult.Fail("This observation does not exist.");
        }

        try
        {
            _repository.CreateComment(observationId, authorId, text.Trim());
            return PostResult.Ok();
        }
        catch (DbUpdateException)
        {
            // Saving failed, so nothing was stored
            return PostResult.Fail("Your comment could not be saved. Please try again.");
        }
    }

    // Validates and stores a new taxon proposal
    public PostResult AddProposal(int observationId, int authorId, int taxonId, string? text)
    {
        // The observation must exist
        if (_repository.GetObservation(observationId) == null)
        {
            return PostResult.Fail("This observation does not exist.");
        }

        // The proposed taxon must exist
        if (_repository.GetTaxon(taxonId) == null)
        {
            return PostResult.Fail("Please choose a valid taxon.");
        }

        try
        {
            // The reason is optional, so store an empty string if none was given
            _repository.CreateProposal(observationId, authorId, taxonId, text?.Trim() ?? "");
            return PostResult.Ok();
        }
        catch (DbUpdateException)
        {
            // Saving failed, so nothing was stored
            return PostResult.Fail("Your proposal could not be saved. Please try again.");
        }
    }
}