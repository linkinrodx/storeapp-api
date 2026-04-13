using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Data;
using StoreApp.Domain.Mappings;
using StoreApp.Domain.Middleware;
using StoreApp.Domain.Repositories.Implementations;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;
using StoreApp.Domain.Services.Interfaces;

namespace StoreApp.Api.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Registra DbContext, AutoMapper, repositorios y servicios de negocio.
    /// Prioriza la variable de ambiente DATABASE_URL para Lambda.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registrar DbContext - priorizar variable de ambiente para Lambda
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not found. Set DATABASE_URL environment variable or configure 'DefaultConnection' in appsettings.json");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.CommandTimeout(30);
                npgsql.EnableRetryOnFailure(3);
            }));

        services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

        // Registrar repositorio genérico y servicios
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IFamilyService, FamilyService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IWishlistService, WishlistService>();

        return services;
    }

    /// <summary>
    /// Configura el pipeline de middleware de la aplicación.
    /// </summary>
    public static void ConfigureApplicationPipeline(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        var enableSwagger = env.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}