using StoreApp.Api.Extensions;
using Microsoft.AspNetCore.CookiePolicy;

namespace StoreApp.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Agregar CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Registrar todos los servicios de la aplicación
        services.AddServices(configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Usar CORS
        app.UseCors("AllowAll");

        // Configurar pipeline de middleware
        app.ConfigureApplicationPipeline(env);
    }
}
