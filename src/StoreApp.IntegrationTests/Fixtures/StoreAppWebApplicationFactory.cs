using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoreApp.Api;
using StoreApp.Domain.Data;

namespace StoreApp.IntegrationTests.Fixtures;

/// <summary>
/// WebApplicationFactory personalizado para integration tests.
/// Configura la aplicación para usar un contenedor PostgreSQL real.
/// </summary>
public class StoreAppWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly TestDatabaseManager _databaseManager;

    public StoreAppWebApplicationFactory(TestDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover el DbContext registrado originalmente
            var dbContextDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Registrar DbContext con la base de datos de test
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    _databaseManager.ConnectionString,
                    npgsql =>
                    {
                        npgsql.CommandTimeout(30);
                        npgsql.EnableRetryOnFailure(3);
                    });
            });
        });

        builder.UseEnvironment("Testing");
    }
}
