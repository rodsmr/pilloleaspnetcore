var builder = WebApplication.CreateBuilder(args);

// Servizi

// Singleton: instanza sarà sempre la stessa per tutta la vita dell'applicazione
builder.Services.AddSingleton<MyService>(); // questa riga mi evita di creare un oggetto MyService ogni volta che viene chiamato il servizio
// Scoped
// Transient
var app = builder.Build();

// Middleware
app.MapGet("/", (MyService myService) => 
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