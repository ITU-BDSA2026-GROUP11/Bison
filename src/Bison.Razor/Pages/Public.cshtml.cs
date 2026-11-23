using Bison.Infastructure.DTO;
using Bison.Infastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Components.Forms;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;

    public List<PostDTO> Observations { get; set; } = new();
    public int CurrentPage { get; set; }

    public readonly IWebHostEnvironment _env;

    public string Text {get; set;}

    // All taxa for the taxon selector
    public List<TaxonDTO> Taxa { get; set; } = new();

    // The taxon currently filtered by, or null when showing all observations
    [BindProperty]public int? SelectedTaxonId { get; set; }

    // Display name of the selected taxon, or null when showing all observations
    public string? SelectedTaxonName { get; set; }

    [BindProperty] public required string InputObs {get; set;} = "";

    [BindProperty] public IFormFile? InputPhoto {get; set;} = null;
    public PublicModel(IObservationService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;

    }

    // Gets the page number and optional taxon from the URL, for example ?page=2&taxon=5
    public ActionResult OnGet([FromQuery] int? page, [FromQuery] int? taxon)
    {
        // If no page is given, use page 1
        CurrentPage = page ?? 1;

        // Prevent page 0 or negative page numbers
        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        Taxa = _service.GetAllTaxa();
        SelectedTaxonId = taxon;

        if (taxon.HasValue)
        {
            // Filter by the taxon and its subtaxa, using the verified filter
            Observations = _service.GetObservationsByTaxon(taxon.Value, CurrentPage);
            SelectedTaxonName = Taxa.FirstOrDefault(t => t.TaxonId == taxon.Value)?.Name;
        }
        else
        {
            // Gets only the observations for the requested page
            Observations = _service.GetObservations(CurrentPage);
        }

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {

        Console.WriteLine("Hello world");
        var currentlyUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string finalPath = "";
        var photoPath = "";
        bool isImgValid = true;

        if (InputPhoto != null)
        {
            isImgValid = isValidImgType(InputPhoto);
            Directory.CreateDirectory("wwwroot/uploads");
            //photoPath = Path.Combine("uploads", InputPhoto.FileName);
            photoPath = "uploads/" + InputPhoto.FileName;
            finalPath = Path.Combine(_env.WebRootPath, photoPath);
            await using var stream = new FileStream(finalPath, FileMode.Create);
            await InputPhoto.CopyToAsync(stream);
        }

        if (SelectedTaxonId == null)
        {
        Taxa = _service.GetAllTaxa();
        ModelState.AddModelError("", "Please select a taxon.");
        return Page();
        }

        int obsLength = InputObs.Length;
        if (isImgValid && !string.IsNullOrEmpty(InputObs) && obsLength < 200)
        {
        await _service.addObservation(InputObs, int.Parse(currentlyUserId), SelectedTaxonId.Value, photoPath);
        }
        return RedirectToPage("Public");
    }

    public bool isValidImgType(IFormFile InputFileImg)
    {
        byte[] checkFileBytes = new byte[8];
        using var stream = InputFileImg.OpenReadStream();
        stream.Read(checkFileBytes, 0, checkFileBytes.Length);

        bool isPng = 
        checkFileBytes[0] == 0x89 &&
        checkFileBytes[1] == 0x50 &&
        checkFileBytes[2] == 0x4E &&
        checkFileBytes[3] == 0x47 &&
        checkFileBytes[4] == 0x0D &&
        checkFileBytes[5] == 0x0A &&
        checkFileBytes[6] == 0x1A &&
        checkFileBytes[7] == 0x0A;

        bool isJpg = 
        checkFileBytes[0] == 0xFF &&
        checkFileBytes[1] == 0xD8 &&
        checkFileBytes[2] == 0xFF;
       
        
        if (!isPng && !isJpg){
        return false;
        }
        return true;
    }

    
    
}