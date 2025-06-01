using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);

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

// Middleware: pezzo di codice che viene eseguito per ogni richiesta HTTP, decidendo se
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


app.MapGet("/", ([FromKeyedServices("myService")]MyService myService) => 
{
    // var myService = new MyService(); // riga rimossa dopo aver registrato il servizio
    // il servizio ora viene iniettato
    return $"Hello World! {myService.GetValue()}";
});

app.Run();

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