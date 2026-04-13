using System.Net;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Orders.
/// Prueba obtener órdenes, filtrado por usuario y estado.
/// </summary>
public class OrdersEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetOrders_WithValidParameters_ReturnsOkStatusAndOrderList()
    {
        // Arrange
        var requestUrl = "/api/Orders?page=1&pageSize=20";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }

    [Theory]
    [InlineData(0)]  // page = 0 (inválido)
    [InlineData(-1)] // page negativo
    public async Task GetOrders_WithInvalidPage_ReturnsBadRequest(int page)
    {
        // Arrange
        var requestUrl = $"/api/Orders?page={page}&pageSize=20";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]   // pageSize = 0 (inválido)
    [InlineData(-10)] // pageSize negativo
    [InlineData(1001)] // pageSize muy grande
    public async Task GetOrders_WithInvalidPageSize_ReturnsBadRequest(int pageSize)
    {
        // Arrange
        var requestUrl = $"/api/Orders?page=1&pageSize={pageSize}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetOrders_WithUserFilter_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestUrl = $"/api/Orders?page=1&pageSize=20&userId={userId}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("completed")]
    [InlineData("cancelled")]
    public async Task GetOrders_WithStatusFilter_ReturnsOk(string status)
    {
        // Arrange
        var requestUrl = $"/api/Orders?page=1&pageSize=20&status={status}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderById_WithValidId_ReturnsOkOrNotFound()
    {
        // Arrange
        var validId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Orders/{validId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrderById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Orders/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrders_WithUserAndStatusFilter_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var status = "completed";
        var requestUrl = $"/api/Orders?page=1&pageSize=20&userId={userId}&status={status}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(10, 50)]
    public async Task GetOrders_Pagination_ReturnsCorrectStatus(int page, int pageSize)
    {
        // Arrange
        var requestUrl = $"/api/Orders?page={page}&pageSize={pageSize}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
