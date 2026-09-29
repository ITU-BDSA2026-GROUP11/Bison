var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

string? databasePath = Environment.GetEnvironmentVariable("BISONDBPATH");

if (string.IsNullOrWhiteSpace(databasePath))
{
    databasePath = Path.Combine(Path.GetTempPath(), "bison.db");
}

builder.Services.AddSingleton(new DBFacade(databasePath));
builder.Services.AddScoped<IObservationService, ObservationService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();