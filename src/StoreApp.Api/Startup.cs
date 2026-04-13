using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Data;
using StoreApp.Domain.Mappings;
using StoreApp.Domain.Middleware;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Interfaces;
using StoreApp.Domain.Services.Implementations;
using StoreApp.Domain.Repositories.Implementations;

namespace StoreApp.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Leer la cadena de conexión - priorizar variable de ambiente para Lambda
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

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        // No llamar a AddServices aquí porque ya estamos registrando el DbContext arriba
        // y evitamos registrarlo dos veces
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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
