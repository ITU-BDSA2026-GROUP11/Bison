using Bison.Razor.Models;

namespace Bison.Razor;

// Wraps the Dafny-generated code so the rest of the app never
// has to deal with Dafny types directly.
public class TaxonFilterService : ITaxonFilterService
{
    public List<Observation> FilterByTaxon(Taxon root, IEnumerable<Observation> observations)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(observations);

        // Convert the C# collection to a Dafny sequence
        var sequence = Dafny.Sequence<Observation>.FromArray(observations.ToArray());

        // Call the verified wrapper method from Filtering.dfy (module FilterWrapper)
        Observation[] filtered = FilterWrapper.__default.FilterObservations(root, sequence);

        return filtered.ToList();
    }
}