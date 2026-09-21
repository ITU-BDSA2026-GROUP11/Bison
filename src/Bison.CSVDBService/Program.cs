var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/read", () => "Hello World!");

app.MapGet("/comment", () => "Hello World!");

app.MapGet("/observe", () => "Hello World!");

app.MapGet("/location", () => "Hello World!");

app.MapGet("/discussion", () => "Hello World!");

//app.MapGet("/observe", () => )


app.Run();
