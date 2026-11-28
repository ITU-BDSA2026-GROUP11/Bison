
using Bison.Core.Models;
using Bison.Infastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bison.Razor.Pages;

[Authorize]
public class AboutMeModel : PageModel
{
    private const string DeletedAuthorEmail =
        "deleted-user@anonymous.invalid";

    private readonly BisonDBContext _db;
    private readonly UserManager<Author> _userManager;
    private readonly SignInManager<Author> _signInManager;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<AboutMeModel> _logger;

    public AboutMeModel(
        BisonDBContext db,
        UserManager<Author> userManager,
        SignInManager<Author> signInManager,
        IWebHostEnvironment environment,
        ILogger<AboutMeModel> logger)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
        _logger = logger;
    }

    public Author CurrentAuthor { get; private set; } = null!;

    public List<Observation> Observations { get; private set; } = new();

    public List<Comment> Comments { get; private set; } = new();

    public List<Proposal> Proposals { get; private set; } = new();

    public IList<UserLoginInfo> Logins { get; private set; } =
        new List<UserLoginInfo>();

    public async Task<IActionResult> OnGetAsync()
    {
        var author = await _userManager.GetUserAsync(User);

        if (author == null)
            return Challenge();

        await LoadInformationAsync(author);

        return Page();
    }

    private async Task LoadInformationAsync(Author author)
    {
        CurrentAuthor = author;

        Observations = await _db.Observations
            .AsNoTracking()
            .Where(o => o.AuthorId == author.Id)
            .OrderByDescending(o => o.TimeStamp)
            .ToListAsync();

        Comments = await _db.Comments
            .AsNoTracking()
            .Where(c => c.AuthorId == author.Id)
            .OrderByDescending(c => c.TimeStamp)
            .ToListAsync();

        Proposals = await _db.Proposals
            .AsNoTracking()
            .Where(p => p.AuthorId == author.Id)
            .OrderByDescending(p => p.TimeStamp)
            .ToListAsync();

        Logins = await _userManager.GetLoginsAsync(author);
    }

    public async Task<IActionResult> OnPostForgetAsync(string? choice)
    {
        var author = await _userManager.GetUserAsync(User);

        if (author == null)
            return Challenge();

        if (choice != "keep" && choice != "delete")
        {
            ModelState.AddModelError(
                "",
                "Please choose what happens to your observations.");

            await LoadInformationAsync(author);

            return Page();
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var observationIds = await _db.Observations
            .Where(o => o.AuthorId == author.Id)
            .Select(o => o.PostId)
            .ToListAsync();

        var imagePaths = choice == "delete"
            ? await _db.Observations
                .Where(o =>
                    o.AuthorId == author.Id &&
                    o.ImgPath != null)
                .Select(o => o.ImgPath!)
                .ToListAsync()
            : new List<string>();

        // Delete the user's comments and proposals.
        await _db.Comments
            .Where(c => c.AuthorId == author.Id)
            .ExecuteDeleteAsync();

        await _db.Proposals
            .Where(p => p.AuthorId == author.Id)
            .ExecuteDeleteAsync();

        if (choice == "keep")
        {
            if (observationIds.Count > 0)
            {
                // Use one shared anonymous author for preserved observations.
                var deletedAuthor = await _db.Authors
                    .FirstOrDefaultAsync(a =>
                        a.Name == "deleted user" &&
                        a.Email == DeletedAuthorEmail &&
                        a.UserName == null);

                if (deletedAuthor == null)
                {
                    deletedAuthor = new Author
                    {
                        Name = "Deleted User",
                        Email = DeletedAuthorEmail
                    };

                    _db.Authors.Add(deletedAuthor);

                    await _db.SaveChangesAsync();
                }

                int deletedAuthorId = deletedAuthor.Id;

                await _db.Observations
                    .Where(o => o.AuthorId == author.Id)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(
                            o => o.AuthorId,
                            deletedAuthorId));
            }
        }
        else
        {
            // Delete observations and their related comments and proposals.
            await _db.Comments
                .Where(c => observationIds.Contains(c.ObservationId))
                .ExecuteDeleteAsync();

            await _db.Proposals
                .Where(p => observationIds.Contains(p.ObservationId))
                .ExecuteDeleteAsync();

            await _db.Observations
                .Where(o => o.AuthorId == author.Id)
                .ExecuteDeleteAsync();
        }

        var result = await _userManager.DeleteAsync(author);

        if (!result.Succeeded)
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(
                "",
                "Could not delete your account. Please try again.");

            await LoadInformationAsync(author);

            return Page();
        }

        await transaction.CommitAsync();

        await _signInManager.SignOutAsync();

        if (choice == "delete")
        {
            foreach (var imagePath in imagePaths.Distinct())
            {
                await DeleteUploadedImageAsync(imagePath);
            }
        }

        return Redirect("/");
    }

    private async Task DeleteUploadedImageAsync(string savedPath)
    {
        if (string.IsNullOrWhiteSpace(_environment.WebRootPath) ||
            string.IsNullOrWhiteSpace(savedPath))
        {
            return;
        }

        var relativePath = Uri.UnescapeDataString(
            savedPath.Split('?', '#')[0])
            .TrimStart('~', '/');

        if (!relativePath.StartsWith(
            "uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var uploadFolder = Path.GetFullPath(
            Path.Combine(_environment.WebRootPath, "uploads"))
            + Path.DirectorySeparatorChar;

        var fullPath = Path.GetFullPath(
            Path.Combine(_environment.WebRootPath, relativePath));

        if (!fullPath.StartsWith(
            uploadFolder, StringComparison.Ordinal))
        {
            return;
        }

        if (await _db.Observations
            .AnyAsync(o => o.ImgPath == savedPath))
        {
            return;
        }

        try
        {
            System.IO.File.Delete(fullPath);
        }
        catch (IOException ex)
        {
            _logger.LogWarning(
                ex,
                "Could not remove image {Path}",
                savedPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                ex,
                "Could not remove image {Path}",
                savedPath);
        }
    }
}
