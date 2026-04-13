using System.Net;
using System.Text;
using System.Text.Json;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Cart.
/// Prueba obtener carrito, agregar items, remover items y limpiar carrito.
/// </summary>
public class CartEndpointsTests : IntegrationTestBase
{
    private readonly Guid _testUserId = Guid.NewGuid();

    [Fact]
    public async Task GetCart_WithValidUserId_ReturnsOkStatus()
    {
        // Arrange
        var requestUrl = $"/api/users/{_testUserId}/cart";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetCart_ReturnsCartItemList()
    {
        // Arrange
        var requestUrl = $"/api/users/{_testUserId}/cart";

        // Act
        var response = await Client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
    }

    [Fact]
    public async Task UpsertCartItem_WithValidProductId_ReturnsOkOrBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var requestUrl = $"/api/users/{_testUserId}/cart";
        var requestBody = new
        {
            productId,
            quantity = 1
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync(requestUrl, jsonContent);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task UpsertCartItem_WithDifferentQuantities_ReturnsOkOrError(int quantity)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var requestUrl = $"/api/users/{_testUserId}/cart";
        var requestBody = new
        {
            productId,
            quantity
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync(requestUrl, jsonContent);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveCartItem_WithValidProductId_ReturnsNoContentOrNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var requestUrl = $"/api/users/{_testUserId}/cart/{productId}";

        // Act
        var response = await Client.DeleteAsync(requestUrl);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent || 
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ClearCart_WithValidUserId_ReturnsNoContent()
    {
        // Arrange
        var requestUrl = $"/api/users/{_testUserId}/cart";

        // Act
        var response = await Client.DeleteAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetCart_WithDifferentUserIds_ReturnsOk()
    {
        // Arrange
        var userIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // Act & Assert
        foreach (var userId in userIds)
        {
            var requestUrl = $"/api/users/{userId}/cart";
            var response = await Client.GetAsync(requestUrl);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task CartOperations_Sequence_ReturnsConsistentStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act - Get cart
        var getResponse = await Client.GetAsync($"/api/users/{userId}/cart");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Act - Try to add item
        var upsertBody = new { productId, quantity = 1 };
        var upsertContent = new StringContent(
            JsonSerializer.Serialize(upsertBody),
            Encoding.UTF8,
            "application/json");
        var upsertResponse = await Client.PostAsync($"/api/users/{userId}/cart", upsertContent);
        Assert.True(
            upsertResponse.StatusCode == HttpStatusCode.OK || 
            upsertResponse.StatusCode == HttpStatusCode.BadRequest ||
            upsertResponse.StatusCode == HttpStatusCode.NotFound);

        // Act - Clear cart
        var clearResponse = await Client.DeleteAsync($"/api/users/{userId}/cart");
        Assert.Equal(HttpStatusCode.NoContent, clearResponse.StatusCode);
    }
}
