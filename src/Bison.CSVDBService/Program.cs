using Bison.CSVDBService;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaxonomyRepository, TaxonomyRepository>();

var app = builder.Build();

app.MapGet("/read", () => "Hello World!");
app.MapGet("/comment", () => "Hello World!");
app.MapGet("/observe", () => "Hello World!");
app.MapGet("/location", () => "Hello World!");
app.MapGet("/discussion", () => "Hello World!");

//app.MapGet("/observe", () => )


// temp test for the taxonomy
// use "dotnet run" while in Bison.CSVDBService
using var scope = app.Services.CreateScope();

var repo = scope.ServiceProvider.GetRequiredService<ITaxonomyRepository>();

Console.WriteLine("Testing taxonomy repository...");

var fiskehejre = repo.GetByVernacularName("Fiskehejre");

if (fiskehejre == null)
{
    Console.WriteLine("Could not find Fiskehejre");
}
else
{
    Console.WriteLine($"Found: {fiskehejre.ScientificName}");

    var parent = repo.GetSupertaxon(fiskehejre.TaxonId);

    if (parent != null)
    {
        Console.WriteLine($"Parent: {parent.ScientificName}");

        var children = repo.GetSubtaxons(parent.TaxonId);
        Console.WriteLine($"Number of subtaxons: {children.Count}");
    }
    else
    {
        Console.WriteLine("No parent found");
    }
}

app.Run();

