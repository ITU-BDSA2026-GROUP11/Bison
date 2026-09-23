namespace Bison.CSVDBService;

public record Taxon(
    string TaxonId,           // dwc:taxonID
    string ScientificName,    // dwc:scientificName
    string? VernacularName,   // dwc:vernacularName (Danish name, nullable)
    string TaxonRank,         // dwc:taxonRank (Order, Family, Genus, Species, etc.)
    string? ParentTaxonId     // dwc:parentNameUsageID (nullable for root nodes)
);
