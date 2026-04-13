using StoreApp.Api.Extensions;

namespace StoreApp.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Agregar CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Registrar todos los servicios de la aplicación
        builder.Services.AddServices(builder.Configuration);

        var app = builder.Build();

        // Usar CORS
        app.UseCors("AllowAll");

        // Configurar pipeline de middleware
        app.ConfigureApplicationPipeline(app.Environment);

        app.Run();
    }
}
