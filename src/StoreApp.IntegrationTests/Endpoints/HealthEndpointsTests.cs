using System.Net;
using System.Text.Json;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Health Check.
/// Verifica que el API esté disponible y sano.
/// </summary>
public class HealthEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task HealthCheck_ReturnsOkStatus()
    {
        // Arrange
        var requestUrl = "/api/Health";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthyStatus()
    {
        // Arrange
        var requestUrl = "/api/Health";

        // Act
        var response = await Client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();
        
        // Parse JSON response
        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;
        var status = root.GetProperty("status").GetString();

        // Assert
        Assert.NotNull(status);
        Assert.Equal("healthy", status);
    }

    [Fact]
    public async Task HealthCheck_ReturnsTimestamp()
    {
        // Arrange
        var requestUrl = "/api/Health";

        // Act
        var response = await Client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();
        
        // Parse JSON response
        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("timestamp", out var timestampElement));
        Assert.NotEqual("", timestampElement.GetString());
    }

    [Fact]
    public async Task HealthCheck_CanBeCalledMultipleTimes()
    {
        // Arrange
        var requestUrl = "/api/Health";

        // Act
        for (int i = 0; i < 5; i++)
        {
            var response = await Client.GetAsync(requestUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task HealthCheck_ReturnsValidJsonResponse()
    {
        // Arrange
        var requestUrl = "/api/Health";

        // Act
        var response = await Client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        try
        {
            JsonDocument.Parse(content);
        }
        catch
        {
            Assert.True(false, "Response is not valid JSON");
        }
    }
}
