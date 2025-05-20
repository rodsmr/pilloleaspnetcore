var builder = WebApplication.CreateBuilder(args);

// Servizi per funzionare
var app = builder.Build();

// Middleware
app.MapGet("/", () => "Hello World!");

app.Run();
