using System.Net;
using System.Text;
using System.Text.Json;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Profiles.
/// Prueba obtener y actualizar perfiles de usuario.
/// </summary>
public class ProfilesEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetProfile_WithValidId_ReturnsOkOrNotFound()
    {
        // Arrange
        var validId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Profiles/{validId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetProfile_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Profiles/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ReturnsOkOrNotFound()
    {
        // Arrange
        var profileId = Guid.NewGuid();
        var requestUrl = $"/api/Profiles/{profileId}";
        var updateData = new
        {
            fullName = "John Doe Updated",
            phone = "+1234567890",
            shippingAddress = "123 Main St",
            shippingCity = "New York",
            shippingReference = "Apt 5B"
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateData),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync(requestUrl, jsonContent);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProfile_WithPartialData_ReturnsOkOrNotFound()
    {
        // Arrange
        var profileId = Guid.NewGuid();
        var requestUrl = $"/api/Profiles/{profileId}";
        var updateData = new
        {
            fullName = "Jane Doe"
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateData),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync(requestUrl, jsonContent);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("")]           // Empty full name
    [InlineData("+1111111111")] // Just phone
    [InlineData("123 Street")]  // Just address
    public async Task UpdateProfile_WithVariousData_ReturnsOkOrNotFound(string fullName)
    {
        // Arrange
        var profileId = Guid.NewGuid();
        var requestUrl = $"/api/Profiles/{profileId}";
        var updateData = new { fullName };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateData),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync(requestUrl, jsonContent);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var requestUrl = $"/api/Profiles/{invalidId}";
        var updateData = new { fullName = "Updated Name" };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateData),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync(requestUrl, jsonContent);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithMultipleDifferentIds_ConsistentBehavior()
    {
        // Arrange
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // Act & Assert
        foreach (var id in ids)
        {
            var response = await Client.GetAsync($"/api/Profiles/{id}");
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task ProfileOperations_Sequence_ReturnsConsistentStatus()
    {
        // Arrange
        var profileId = Guid.NewGuid();

        // Act - Get profile
        var getResponse = await Client.GetAsync($"/api/Profiles/{profileId}");
        Assert.True(
            getResponse.StatusCode == HttpStatusCode.OK || 
            getResponse.StatusCode == HttpStatusCode.NotFound);

        // Act - Update profile
        var updateData = new { fullName = "Updated User" };
        var updateContent = new StringContent(
            JsonSerializer.Serialize(updateData),
            Encoding.UTF8,
            "application/json");
        var updateResponse = await Client.PutAsync($"/api/Profiles/{profileId}", updateContent);
        Assert.True(
            updateResponse.StatusCode == HttpStatusCode.OK || 
            updateResponse.StatusCode == HttpStatusCode.NotFound);
    }
}
