using Bison.Razor;
using Bison.Razor.Repositories;
using Microsoft.EntityFrameworkCore;

// Create the web application
var builder = WebApplication.CreateBuilder(args);


// Get the database connection string
string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string not found.");


// Register EF Core with SQLite
builder.Services.AddDbContext<BisonDBContext>(options =>
    options.UseSqlite(connectionString));


// Add Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Public", "obs");
    options.Conventions.AddPageRoute("/Public", "ob");
});


// Register the repository
builder.Services.AddScoped<IPostRepository, PostRepository>();


// Register the observation service
builder.Services.AddScoped<IObservationService, ObservationService>();


// Build the application
var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<BisonDBContext>();

    // Apply database migrations
    context.Database.Migrate();

    // Insert the data from DbInitializer.cs
    DbInitializer.SeedDatabase(context);

    // Check that data was inserted
    Console.WriteLine("DATABASE INITIALIZED");

    Console.WriteLine($"Authors: {context.Authors.Count()}");
    Console.WriteLine($"Taxons: {context.Taxons.Count()}");
    Console.WriteLine($"Observations: {context.Observations.Count()}");
    Console.WriteLine($"Comments: {context.Comments.Count()}");
    Console.WriteLine($"Proposals: {context.Proposals.Count()}");
}

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


// HTTPS
app.UseHttpsRedirection();


// Static files
app.UseStaticFiles();


// Routing
app.UseRouting();


// Razor Pages
app.MapRazorPages();


// Start application
app.Run();


public partial class Program { }