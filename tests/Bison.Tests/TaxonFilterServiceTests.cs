using Bison.Razor;
using Bison.Razor.Models;

namespace Bison.Tests;

public class TaxonFilterServiceTests
{
    // Test taxonomy:
    //
    // Animalia (1)
    // └── Chordata (2)
    //     ├── Mammalia (3)
    //     │   └── Bison (4)
    //     └── Aves (5)
    // Plantae (6)              <- separate tree
    private readonly Taxon _animalia;
    private readonly Taxon _chordata;
    private readonly Taxon _mammalia;
    private readonly Taxon _bison;
    private readonly Taxon _aves;
    private readonly Taxon _plantae;

    private readonly TaxonFilterService _service = new();

    public TaxonFilterServiceTests()
    {
        _animalia = CreateTaxon(1, "Animalia", null);
        _chordata = CreateTaxon(2, "Chordata", _animalia);
        _mammalia = CreateTaxon(3, "Mammalia", _chordata);
        _bison = CreateTaxon(4, "Bison", _mammalia);
        _aves = CreateTaxon(5, "Aves", _chordata);
        _plantae = CreateTaxon(6, "Plantae", null);
    }

    private static Taxon CreateTaxon(int id, string name, Taxon? parent)
    {
        var taxon = new Taxon
        {
            TaxonId = id,
            VernacularName = name,
            Parent = parent,
            ParentId = parent?.TaxonId
        };
        parent?.Children.Add(taxon);
        return taxon;
    }

    private static Observation CreateObservation(Taxon taxon)
    {
        return new Observation
        {
            Taxon = taxon,
            TaxonId = taxon.TaxonId
        };
    }

    [Fact]
    public void FilterByLeafTaxon_ReturnsOnlyThatTaxon()
    {
        var bisonObs = CreateObservation(_bison);
        var avesObs = CreateObservation(_aves);

        var result = _service.FilterByTaxon(_bison, new[] { bisonObs, avesObs });

        Assert.Single(result);
        Assert.Same(bisonObs, result[0]);
    }

    [Fact]
    public void FilterByIntermediateTaxon_IncludesSubtaxa()
    {
        var bisonObs = CreateObservation(_bison);
        var mammaliaObs = CreateObservation(_mammalia);
        var avesObs = CreateObservation(_aves);

        var result = _service.FilterByTaxon(_mammalia, new[] { bisonObs, mammaliaObs, avesObs });

        Assert.Equal(2, result.Count);
        Assert.Contains(bisonObs, result);
        Assert.Contains(mammaliaObs, result);
        Assert.DoesNotContain(avesObs, result);
    }

    [Fact]
    public void FilterByRootTaxon_ReturnsWholeTree()
    {
        var bisonObs = CreateObservation(_bison);
        var avesObs = CreateObservation(_aves);
        var animaliaObs = CreateObservation(_animalia);
        var plantObs = CreateObservation(_plantae);

        var result = _service.FilterByTaxon(_animalia, new[] { bisonObs, avesObs, animaliaObs, plantObs });

        Assert.Equal(3, result.Count);
        Assert.DoesNotContain(plantObs, result);
    }

    [Fact]
    public void FilterWithNoMatches_ReturnsEmptyList()
    {
        var bisonObs = CreateObservation(_bison);
        var avesObs = CreateObservation(_aves);

        var result = _service.FilterByTaxon(_plantae, new[] { bisonObs, avesObs });

        Assert.Empty(result);
    }

    [Fact]
    public void FilterEmptyInput_ReturnsEmptyList()
    {
        var result = _service.FilterByTaxon(_animalia, Array.Empty<Observation>());

        Assert.Empty(result);
    }

    [Fact]
    public void FilterDoesNotIncludeParentTaxon()
    {
        // Filtering by Bison must not include an observation of its parent Mammalia
        var mammaliaObs = CreateObservation(_mammalia);

        var result = _service.FilterByTaxon(_bison, new[] { mammaliaObs });

        Assert.Empty(result);
    }

    [Fact]
    public void FilterPreservesOrder()
    {
        var first = CreateObservation(_bison);
        var skipped = CreateObservation(_plantae);
        var second = CreateObservation(_aves);
        var third = CreateObservation(_mammalia);

        var result = _service.FilterByTaxon(_chordata, new[] { first, skipped, second, third });

        Assert.Equal(new[] { first, second, third }, result);
    }

    [Fact]
    public void FilterNullRoot_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.FilterByTaxon(null!, Array.Empty<Observation>()));
    }
}