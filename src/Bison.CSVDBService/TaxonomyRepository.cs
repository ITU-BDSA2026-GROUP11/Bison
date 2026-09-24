using System.Reflection;
using System.Text;

namespace Bison.CSVDBService;

public interface ITaxonomyRepository
{
    Taxon? GetById(string taxonId);
    Taxon? GetByVernacularName(string name);
    Taxon? GetSupertaxon(string taxonId);
    IReadOnlyList<Taxon> GetSubtaxons(string taxonId);
}

public class TaxonomyRepository : ITaxonomyRepository
{
    // Quick lookup tables
    private readonly Dictionary<string, Taxon> byId =
        new(StringComparer.Ordinal);

    private readonly Dictionary<string, Taxon> byName =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, List<Taxon>> childrenByParent =
        new(StringComparer.Ordinal);

    public TaxonomyRepository()
    {
        LoadData();
    }

    private void LoadData()
    {
        using var stream = GetTaxonStream();
        using var reader = new StreamReader(stream);

        var header = reader.ReadLine();
        if (header is null)
            return;

        // Support both CSV and tab-separated files
        char separator = header.Contains('\t') ? '\t' : ',';
        var columns = ParseLine(header, separator);

        int idCol = columns.IndexOf("dwc:taxonID");
        int sciCol = columns.IndexOf("dwc:scientificName");
        int nameCol = columns.IndexOf("dwc:vernacularName");
        int rankCol = columns.IndexOf("dwc:taxonRank");
        int parentCol = columns.IndexOf("dwc:parentNameUsageID");

        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = ParseLine(line, separator);

            string id = GetValue(values, idCol);
            if (string.IsNullOrEmpty(id))
                continue;

            var taxon = new Taxon(
                id,
                GetValue(values, sciCol),
                GetOptionalValue(values, nameCol),
                GetValue(values, rankCol),
                GetOptionalValue(values, parentCol)
            );

            // Save taxon by ID
            byId[taxon.TaxonId] = taxon;

            // Save taxon by Danish name
            if (!string.IsNullOrEmpty(taxon.VernacularName))
                byName[taxon.VernacularName] = taxon;

            // Add taxon to its parent's children
            if (!string.IsNullOrEmpty(taxon.ParentTaxonId))
            {
                if (!childrenByParent.TryGetValue(taxon.ParentTaxonId, out var children))
                {
                    children = [];
                    childrenByParent[taxon.ParentTaxonId] = children;
                }

                children.Add(taxon);
            }
        }
    }

    private static Stream GetTaxonStream()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "Bison.CSVDBService.taxons.joined.csv";

        return assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Could not find embedded resource '{resourceName}'.");
    }

    // Get a required value from a row
    private static string GetValue(List<string> values, int index)
    {
        return index >= 0 && index < values.Count
            ? values[index].Trim()
            : string.Empty;
    }

    // Get a value or return null if it is empty
    private static string? GetOptionalValue(List<string> values, int index)
    {
        string value = GetValue(values, index);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public Taxon? GetById(string taxonId)
    {
        return byId.GetValueOrDefault(taxonId);
    }

    public Taxon? GetByVernacularName(string name)
    {
        return byName.GetValueOrDefault(name);
    }

    public Taxon? GetSupertaxon(string taxonId)
    {
        var taxon = GetById(taxonId);

        return taxon?.ParentTaxonId is null
            ? null
            : GetById(taxon.ParentTaxonId);
    }

    public IReadOnlyList<Taxon> GetSubtaxons(string taxonId)
    {
        return childrenByParent.TryGetValue(taxonId, out var children)
            ? children
            : Array.Empty<Taxon>();
    }

    // Split a CSV or TSV row while respecting quotes
    private static List<string> ParseLine(string line, char separator)
    {
        var values = new List<string>();
        var value = new StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // "" inside a quoted value means one quote
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    value.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == separator && !insideQuotes)
            {
                values.Add(value.ToString());
                value.Clear();
            }
            else
            {
                value.Append(c);
            }
        }

        values.Add(value.ToString());
        return values;
    }
}