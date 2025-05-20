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

// Middleware
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