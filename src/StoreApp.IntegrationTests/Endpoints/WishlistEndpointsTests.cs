using System.Net;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Wishlist.
/// Prueba obtener, agregar y remover items de la lista de deseos.
/// </summary>
public class WishlistEndpointsTests : IntegrationTestBase
{
    private readonly Guid _testUserId = Guid.NewGuid();

    [Fact]
    public async Task GetWishlist_WithValidUserId_ReturnsOkStatus()
    {
        // Arrange
        var requestUrl = $"/api/users/{_testUserId}/wishlist";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetWishlist_ReturnsWishlistItemList()
    {
        // Arrange
        var requestUrl = $"/api/users/{_testUserId}/wishlist";

        // Act
        var response = await Client.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
    }

    [Fact]
    public async Task AddToWishlist_WithValidProductId_ReturnsOkOrBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var requestUrl = $"/api/users/{_testUserId}/wishlist/{productId}";

        // Act
        var response = await Client.PostAsync(requestUrl, null);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveFromWishlist_WithValidProductId_ReturnsNoContentOrNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var requestUrl = $"/api/users/{_testUserId}/wishlist/{productId}";

        // Act
        var response = await Client.DeleteAsync(requestUrl);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent || 
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWishlist_WithDifferentUserIds_ReturnsOk()
    {
        // Arrange
        var userIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // Act & Assert
        foreach (var userId in userIds)
        {
            var requestUrl = $"/api/users/{userId}/wishlist";
            var response = await Client.GetAsync(requestUrl);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task AddToWishlist_WithMultipleProducts_ReturnsOkOrError(int productCount)
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        for (int i = 0; i < productCount; i++)
        {
            var productId = Guid.NewGuid();
            var requestUrl = $"/api/users/{userId}/wishlist/{productId}";
            var response = await Client.PostAsync(requestUrl, null);
            
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task WishlistOperations_Sequence_ReturnsConsistentStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act - Get wishlist
        var getResponse = await Client.GetAsync($"/api/users/{userId}/wishlist");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Act - Add to wishlist
        var addResponse = await Client.PostAsync($"/api/users/{userId}/wishlist/{productId}", null);
        Assert.True(
            addResponse.StatusCode == HttpStatusCode.OK || 
            addResponse.StatusCode == HttpStatusCode.BadRequest ||
            addResponse.StatusCode == HttpStatusCode.NotFound);

        // Act - Remove from wishlist
        var removeResponse = await Client.DeleteAsync($"/api/users/{userId}/wishlist/{productId}");
        Assert.True(
            removeResponse.StatusCode == HttpStatusCode.NoContent || 
            removeResponse.StatusCode == HttpStatusCode.NotFound);

        // Act - Get wishlist again
        var getFinalResponse = await Client.GetAsync($"/api/users/{userId}/wishlist");
        Assert.Equal(HttpStatusCode.OK, getFinalResponse.StatusCode);
    }

    [Fact]
    public async Task RemoveFromWishlist_WithNonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var requestUrl = $"/api/users/{userId}/wishlist/{productId}";

        // Act
        var response = await Client.DeleteAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
