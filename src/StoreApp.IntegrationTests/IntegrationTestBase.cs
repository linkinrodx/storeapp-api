using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoreApp.Domain.Data;

namespace StoreApp.IntegrationTests;

/// <summary>
/// Clase base para todos los integration tests.
/// Gestiona el WebApplicationFactory, migraciones y limpieza de datos.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected Fixtures.TestDatabaseManager DatabaseManager { get; private set; } = null!;
    protected Fixtures.StoreAppWebApplicationFactory Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;

    /// <summary>
    /// Se ejecuta antes de cada test.
    /// Inicia la BD de test, crea el factory y aplica migraciones.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        // Inicializar DatabaseManager
        DatabaseManager = new Fixtures.TestDatabaseManager();
        await DatabaseManager.InitializeAsync();

        // Crear WebApplicationFactory
        Factory = new Fixtures.StoreAppWebApplicationFactory(DatabaseManager);
        Client = Factory.CreateClient();

        // Aplicar migraciones a la BD de test
        await ApplyMigrationsAsync();
    }

    /// <summary>
    /// Se ejecuta después de cada test.
    /// Limpia recursos.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await DatabaseManager.DisposeAsync();
    }

    /// <summary>
    /// Aplica todas las migraciones pendientes a la BD de test.
    /// </summary>
    private async Task ApplyMigrationsAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Aplicar migraciones
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Limpia todos los datos de las tablas.
    /// Useful para resetear el estado entre tests.
    /// </summary>
    protected async Task ClearDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Desactivar restricciones de clave foránea durante la limpieza
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE brands CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE families CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE categories CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE products CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE images CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE wishlist CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE cart_items CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE orders CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE order_items CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE profiles CASCADE");
    }

    /// <summary>
    /// Obtiene instancia del DbContext para setup de datos.
    /// </summary>
    protected AppDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
