using StoreApp.Api.Extensions;

namespace StoreApp.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Registrar todos los servicios de la aplicación
        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        // Configurar el pipeline de middleware
        app.ConfigureApplicationPipeline(app.Environment);

        app.Run();
    }
}
