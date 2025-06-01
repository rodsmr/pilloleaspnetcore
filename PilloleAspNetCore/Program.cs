using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Posso utilizzare anche i Services, per mappare su un oggetto: OPTION PATTERN
builder.Services.Configure<ConfigurationObject>(
    builder.Configuration.GetSection("ConfigurationObject")
);

// Servizi
// questa riga mi evita di creare un oggetto MyService ogni volta che viene chiamato il servizio

// Singleton: instanza sarà sempre la stessa per tutta la vita dell'applicazione
//builder.Services.AddSingleton<MyService>(); 

// Scoped: instanza cambia ad ogni richiesta HTTP ma,
// all'interno della stessa richiesta HTTP, l'istanza sarà la stessa
//builder.Services.AddScoped<MyService>();

// Transient: instanza sarà sempre diversa
//builder.Services.AddTransient<MyService>();

// KeyedScoped: comodo se voglio differenziare implementazioni di un servizio
// 1. devo dare un nome all'istanza
// 2. devo decorare la classe MyService con l'attributo KeyedService
builder.Services.AddKeyedScoped<MyService>("myService");

var app = builder.Build();

// Middleware
// Pezzo di codice che viene eseguito per ogni richiesta HTTP, decidendo se
// interrompere la richiesta o farla proseguire: pipeline di esecuzione delle richieste HTTP

// Middleware modo 1
//app.Use(async(context, next) => {

//    if (context.Request.Headers["X-MyHeader"].Contains("test"))
//    {
//        // Interrompe la richiesta e restituisce un messaggio
//        await context.Response.WriteAsync("Richiesta interrotta!");
//        return;
//    }

//    await next(context);
//});

// Middleware modo 2: utilizzare una classe
//app.UseMiddleware<MyMiddleware>();

// Middleware tips: per pulizia, UseMiddleware<MyMiddleware>() va in exentesion method
// tramite classe statica MyMiddlewareExtensions. Quindi non userò più UseMiddleware<MyMiddleware>(),
// bensì app.UseMyMiddleware();
app.UseMyMiddleware();

//app.MapGet("/", ([FromKeyedServices("myService")] MyService myService) =>
//{
//    // var myService = new MyService(); // riga rimossa dopo aver registrato il servizio
//    // il servizio ora viene iniettato
//    return $"Hello World! {myService.GetValue()}";
//});

// Inietto IConfiguration per leggere le configurazioni: è un registrata di default.
// Leggo le configurazioni come se fossero dizionari
//app.MapGet("/", (IConfiguration config) =>
//{
//    return $"Hello World! {config["MyConfiguration"]}";
//});

// appsettings.Development.json dipende da ASPNETCORE_ENVIRONMENT (variabile di ambiente)
// Altro modo per le configurazsioni sono gli UserSecrets: project > Manage User Secrets.
// Sono salvati in AppData del PC e non vanno nel repo

// Altro modo sono le variabili di ambiente, definite in launchSettings.json
//app.MapGet("/", (IConfiguration config) =>
//{
//    return $"Hello World! {config["MY_CONFIGURATION"]}";
//});

// Posso anche utilizzare SECTION per mappare le configurazioni su delle classi
//app.MapGet("/", (IConfiguration config) =>
//{
//    var configurationSection = config.GetSection("ConfigurationObject");
//    return $"Hello World! {configurationSection["Name"]} - {configurationSection["Value"]}";
//});

// Sempre utilizzando SECTION, posso fare binding su un oggetto
//var configurationObject = new ConfigurationObject();
//builder.Configuration
//    .GetSection("ConfigurationObject")
//    .Bind(configurationObject);

//app.MapGet("/", (IConfiguration config) =>
//{
//    return $"Hello World! {configurationObject.Name} - {configurationObject.Value}";
//});

app.MapGet("/", (IOptions<ConfigurationObject> configurationOptions) =>
{
    return $"Hello World! {configurationOptions.Value.Name} - {configurationOptions.Value.Value}";
});

app.Run();

record ConfigurationObject
{
    public string Name { get; set; }
    public string Value { get; set; }
}

#region Service
public class MyService
{
    private static int value = 0;

    public MyService()
    {
        value++;
    }

    public int GetValue()
    {
        return value;
    }
}
#endregion

class MyMiddleware
{
    private readonly RequestDelegate _next;
    public MyMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        // Logica del middleware
        if (context.Request.Headers["X-MyHeader"].Contains("test"))
        {
            // Interrompe la richiesta e restituisce un messaggio
            await context.Response.WriteAsync("Richiesta interrotta!");
            return;
        }
        await _next(context);
    }
}

static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyMiddleware>();
    }
}