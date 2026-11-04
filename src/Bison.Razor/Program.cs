using Bison.Razor;
using Bison.Razor.Models;
using Bison.Razor.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Create the web application
var builder = WebApplication.CreateBuilder(args);

// Database connection
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection string not found.");

// Register EF Core with SQLite
builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite(connectionString));

// Register ASP.NET Core Identity
builder.Services.AddDefaultIdentity<Author>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // Password requirements
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<BisonDBContext>();

// Read GitHub OAuth configuration
string? githubClientId = builder.Configuration["authentication:github:clientId"];
string? githubClientSecret = builder.Configuration["authentication:github:clientSecret"];

// Add GitHub login when OAuth credentials are configured
if (!string.IsNullOrWhiteSpace(githubClientId) && !string.IsNullOrWhiteSpace(githubClientSecret))
{
    builder.Services.AddAuthentication().AddGitHub(options =>
    {
        options.ClientId = githubClientId;
        options.ClientSecret = githubClientSecret;
        options.CallbackPath = "/signin-github";
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.Scope.Add("user:email");
    });
}

// Add Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Public", "obs");
    options.Conventions.AddPageRoute("/Public", "ob");
    options.Conventions.AddPageRoute("/UserTimeline", "obs/{author}");
});

// Register existing repositories and services
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITaxonFilterService, TaxonFilterService>();
builder.Services.AddScoped<IObservationService, ObservationService>();

// Build application
var app = builder.Build();

// Initialize data after applying the reviewed migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
    
    context.Database.Migrate();
    DbInitializer.SeedDatabase(context);

    // Create the two required users locally
    if (app.Environment.IsDevelopment())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();

        await EnsureCourseAuthorAsync(context, userManager, "Eduard", "edka@itu.dk", "123456");
        await EnsureCourseAuthorAsync(context, userManager, "Debasmita", "debloh@itu.dk", "password");
    }

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authenticate users before checking authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable Razor Pages
app.MapRazorPages();

// Start Bison
app.Run();

static async Task EnsureCourseAuthorAsync(BisonDBContext context, UserManager<Author> userManager, string name, string email, string password)
{
    var author = await context.Authors.SingleOrDefaultAsync(a => a.Email == email);

    if (author is null)
    {
        author = new Author
        {
            Name = name,
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(author, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        return;
    }

    // Existing author: preserve ID and associated posts
    author.UserName = email;
    author.EmailConfirmed = true;

    if (string.IsNullOrWhiteSpace(author.Name))
    {
        author.Name = name;
    }

    var updateResult = await userManager.UpdateAsync(author);

    if (!updateResult.Succeeded)
    {
        throw new InvalidOperationException(string.Join("; ", updateResult.Errors.Select(e => e.Description)));
    }

    // Give the existing author an Identity password
    if (!await userManager.HasPasswordAsync(author))
    {
        var passwordResult = await userManager.AddPasswordAsync(author, password);

        if (!passwordResult.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", passwordResult.Errors.Select(e => e.Description)));
        }
    }
    else if (!await userManager.CheckPasswordAsync(author, password))
    {
        Console.WriteLine($"WARNING: {email} already has a different password.");
    }
}

public partial class Program { }