using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Data;
using StoreApp.Api.Extensions;
using StoreApp.Domain.Mappings;
using StoreApp.Domain.Middleware;

namespace StoreApp.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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

        services.AddServices(configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var enableSwagger = env.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreApp API v1");
            });
        }

        app.UseCors();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
