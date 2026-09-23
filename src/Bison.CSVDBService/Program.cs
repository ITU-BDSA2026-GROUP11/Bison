var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/observation", () => "Hello World!");

app.MapPost("/observations", () => "Hello World!");

app.MapGet("/comment", () => "Hello World!");

app.MapGet("/comments", () => "Hello World!");



//app.MapGet("/observe", () => )


app.Run();
