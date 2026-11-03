using Bison.Razor.DTO;
using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;

namespace Bison.Razor.Repositories;

// Handles database access for observations, comments, and proposals
public class PostRepository : IPostRepository
{
    // Database context
    private readonly BisonDBContext _context;

    // Get database context through dependency injection
    public PostRepository(BisonDBContext context)
    {
        _context = context;
    }


    // Get observations for the public timeline
    public List<PostDTO> GetObservations(int page, int pageSize)
    {
        int offset = (page - 1) * pageSize;

        var observations = _context.Observations
            .Include(o => o.Author)
            .OrderByDescending(o => o.TimeStamp)
            .ThenByDescending(o => o.PostId)
            .Skip(offset)
            .Take(pageSize)
            .ToList();

        return observations.Select(o => new PostDTO
        {
            PostId = o.PostId,
            Author = o.Author.Name,
            Text = o.Text,
            TimeStamp = o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        }).ToList();
    }


    // Get observations from one specific author
    public List<PostDTO> GetObservationsFromAuthor(
        string author,
        int page,
        int pageSize)
    {
        int offset = (page - 1) * pageSize;

        var observations = _context.Observations
            .Include(o => o.Author)
            .Where(o => o.Author.Name == author)
            .OrderByDescending(o => o.TimeStamp)
            .ThenByDescending(o => o.PostId)
            .Skip(offset)
            .Take(pageSize)
            .ToList();

        return observations.Select(o => new PostDTO
        {
            PostId = o.PostId,
            Author = o.Author.Name,
            Text = o.Text,
            TimeStamp = o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        }).ToList();
    }


    // Get one observation by its ID
    public PostDTO? GetObservation(int id)
    {
        var observation = _context.Observations
            .Include(o => o.Author)
            .FirstOrDefault(o => o.PostId == id);

        if (observation == null)
        {
            return null;
        }

        return new PostDTO
        {
            PostId = observation.PostId,
            Author = observation.Author.Name,
            Text = observation.Text,
            TimeStamp = observation.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        };
    }


    // Get comments belonging to an observation
    public List<PostDTO> GetComments(int observationId)
    {
        var comments = _context.Comments
            .Include(c => c.Author)
            .Where(c => c.ObservationId == observationId)
            .OrderBy(c => c.TimeStamp)
            .ToList();

        return comments.Select(c => new PostDTO
        {
            PostId = c.PostId,
            Author = c.Author.Name,
            Text = c.Text,
            TimeStamp = c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        }).ToList();
    }


    // Get proposals belonging to an observation
    public List<proposalDTO> GetProposals(int observationId)
    {
        var proposals = _context.Proposals
            .Include(p => p.Author)
            .Where(p => p.ObservationId == observationId)
            .OrderBy(p => p.TimeStamp)
            .ToList();

        return proposals.Select(p => new proposalDTO
        {
            TaxonId = p.TaxonId,
            Author = p.Author.Name,
            Text = p.Text,
            TimeStamp = p.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        }).ToList();
    }


    // Get all observations with their taxon and the taxon's full parent chain
    public List<Observation> GetAllObservationsWithTaxa()
    {
        // Load every taxon into the context. EF Core then links each
        // taxon's Parent to the already-loaded parent object, so the
        // whole chain up to the root is available for IsSubTaxon.
        _context.Taxons.Load();

        return _context.Observations
            .Include(o => o.Author)
            .Include(o => o.Taxon)
            .OrderByDescending(o => o.TimeStamp)
            .ThenByDescending(o => o.PostId)
            .ToList();
    }


    // Get one taxon by its ID
    public Taxon? GetTaxon(int taxonId)
    {
        return _context.Taxons.Find(taxonId);
    }


    // Get all taxa for the taxon selector, sorted by display name
    public List<TaxonDTO> GetAllTaxa()
    {
        return _context.Taxons
            .AsEnumerable()
            .Select(t => new TaxonDTO
            {
                TaxonId = t.TaxonId,
                // Some taxa have no vernacular name, so fall back to the Darwin Core id
                Name = t.VernacularName ?? t.dwc_TaxonID ?? $"Taxon {t.TaxonId}"
            })
            .OrderBy(t => t.Name)
            .ToList();
    }
}