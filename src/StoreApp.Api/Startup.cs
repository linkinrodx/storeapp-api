using StoreApp.Api.Extensions;

namespace StoreApp.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Registrar todos los servicios de la aplicación
        services.AddApplicationServices(configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configurar el pipeline de middleware
        app.ConfigureApplicationPipeline(env);
    }
}
