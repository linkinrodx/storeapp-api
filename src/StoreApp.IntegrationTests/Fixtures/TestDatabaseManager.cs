using Testcontainers.PostgreSql;

namespace StoreApp.IntegrationTests.Fixtures;

/// <summary>
/// Gestiona el contenedor PostgreSQL para integration tests.
/// Inicia un contenedor PostgreSQL real para cada suite de tests.
/// </summary>
public class TestDatabaseManager : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private string _connectionString = string.Empty;

    public string ConnectionString => _connectionString;

    /// <summary>
    /// Inicia el contenedor PostgreSQL con la configuración necesaria.
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("storeapp_test")
            .WithUsername("postgres")
            .WithPassword("postgres_test_123")
            .WithPortBinding(5433, 5432) // Puerto diferente al de desarrollo
            .Build();

        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();
    }

    /// <summary>
    /// Detiene y elimina el contenedor PostgreSQL.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}
