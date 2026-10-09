using Bison.Razor;
using Bison.Razor.Repositories;
using Microsoft.EntityFrameworkCore;

// Creates the web application
var builder = WebApplication.CreateBuilder(args);

//Register database with dbContext
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite(connectionString));

// Adds Razor Pages to the application.
// The public timeline is reachable at "/" (from its @page line),
// and also at "/obs" and "/ob" (an /ob request without an id lists all observations).
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Public", "obs");
    options.Conventions.AddPageRoute("/Public", "ob");
});

// Tries to get the database path from the BISONDBPATH environment variable
string? databasePath = Environment.GetEnvironmentVariable("BISONDBPATH");

// If BISONDBPATH is not set, use bison.db in the temporary folder
if (string.IsNullOrWhiteSpace(databasePath))
{
    databasePath = Path.Combine(Path.GetTempPath(), "bison.db");
}

// Makes DBFacade available through dependency injection
builder.Services.AddSingleton(new DBFacade(databasePath));

// Makes PostRepository available when IPostRepository is requested
builder.Services.AddScoped<IPostRepository, PostRepository>();


// Makes ObservationService available when IObservationService is requested
builder.Services.AddScoped<IObservationService, ObservationService>();

// Builds the application
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Shows an error page if something goes wrong
    app.UseExceptionHandler("/Error");

    // Adds HTTP Strict Transport Security
    app.UseHsts();
}

// Redirects HTTP requests to HTTPS
app.UseHttpsRedirection();

// Allows the app to use static files like CSS and images
app.UseStaticFiles();

// Enables routing
app.UseRouting();

// Connects Razor Pages to their routes
app.MapRazorPages();

// Starts the application
app.Run();


public partial class Program{}