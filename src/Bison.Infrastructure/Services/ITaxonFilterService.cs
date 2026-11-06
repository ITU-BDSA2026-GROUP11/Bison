using Bison.Core.Models;

namespace Bison.Infastructure;

public interface ITaxonFilterService
{
    // Returns the observations whose taxon is root or a subtaxon of root,
    // in the same order as the input. Uses the Dafny-verified filter.
    List<Observation> FilterByTaxon(Taxon root, IEnumerable<Observation> observations);
}