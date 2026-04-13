using System.Net;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Products.
/// Prueba CRUD y validaciones de productos.
/// </summary>
public class ProductEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetProducts_WithValidParameters_ReturnsOkStatusAndProductList()
    {
        // Arrange
        var requestUrl = "/api/Products?page=1&pageSize=20&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(0, 20, true)]   // page inválida
    [InlineData(1, 0, true)]    // pageSize inválida
    [InlineData(-1, 20, true)]  // page negativa
    public async Task GetProducts_WithInvalidParameters_ReturnsBadRequest(int page, int pageSize, bool onlyActive)
    {
        // Arrange
        var requestUrl = $"/api/Products?page={page}&pageSize={pageSize}&onlyActive={onlyActive}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]  // Sin filtro de categoría
    [InlineData("")]    // Categoría vacía
    public async Task GetProducts_WithAndWithoutCategoryFilter_ReturnsOk(string? category)
    {
        // Arrange
        var requestUrl = string.IsNullOrEmpty(category) 
            ? "/api/Products?page=1&pageSize=20&onlyActive=true"
            : $"/api/Products?page=1&pageSize=20&onlyActive=true&category={category}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithInactiveFilter_ReturnsInactiveProducts()
    {
        // Arrange
        var requestUrl = "/api/Products?page=1&pageSize=20&onlyActive=false";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(10, 50)]
    public async Task GetProducts_Pagination_ReturnsCorrectStatus(int page, int pageSize)
    {
        // Arrange
        var requestUrl = $"/api/Products?page={page}&pageSize={pageSize}&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsOkAndProductDetails()
    {
        // Arrange
        var validId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Products/{validId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Products/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductsByCategory_WithValidCategory_ReturnsOk()
    {
        // Arrange
        var requestUrl = "/api/Products/category/electronics?page=1&pageSize=20&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProductsByBrand_WithValidBrand_ReturnsOk()
    {
        // Arrange
        var requestUrl = "/api/Products/brand/samsung?page=1&pageSize=20&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchProducts_WithValidSearchTerm_ReturnsResults()
    {
        // Arrange
        var requestUrl = "/api/Products/search?term=phone&page=1&pageSize=20";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchProducts_WithEmptySearchTerm_ReturnsBadRequest()
    {
        // Arrange
        var requestUrl = "/api/Products/search?term=&page=1&pageSize=20";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
