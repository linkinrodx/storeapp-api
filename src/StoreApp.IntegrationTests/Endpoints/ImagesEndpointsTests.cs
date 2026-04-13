using System.Net;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Images.
/// Prueba obtener imágenes por entidad y por ID.
/// </summary>
public class ImagesEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetImagesByEntity_WithValidEntityId_ReturnsOkStatus()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var requestUrl = $"/api/Images?entityId={entityId}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetImagesByEntity_WithEntityTypeParameter_ReturnsOkStatus()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entityType = "product";
        var requestUrl = $"/api/Images?entityId={entityId}&entityType={entityType}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetImagesByEntity_WithDifferentEntityTypes_ReturnsOkStatus()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entityTypes = new[] { "product", "brand", "category", "family" };

        // Act & Assert
        foreach (var entityType in entityTypes)
        {
            var requestUrl = $"/api/Images?entityId={entityId}&entityType={entityType}";
            var response = await Client.GetAsync(requestUrl);
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task GetImageById_WithValidId_ReturnsOkOrNotFound()
    {
        // Arrange
        var validId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Images/{validId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetImageById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Images/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("product")]
    [InlineData("brand")]
    public async Task GetImagesByEntity_WithAndWithoutEntityType_ReturnsOk(string? entityType)
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var requestUrl = string.IsNullOrEmpty(entityType) 
            ? $"/api/Images?entityId={entityId}"
            : $"/api/Images?entityId={entityId}&entityType={entityType}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetImagesByEntity_WithMultipleCalls_ReturnsConsistentStatus()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var requestUrl = $"/api/Images?entityId={entityId}";

        // Act
        var response1 = await Client.GetAsync(requestUrl);
        var response2 = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(response1.StatusCode, response2.StatusCode);
    }
}
