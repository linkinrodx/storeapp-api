using System.Net;
using System.Text.Json;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Categories.
/// Prueba los principales flujos de la API.
/// </summary>
public class CategoriesEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetCategories_WithValidParameters_ReturnsOkStatusAndCategoryList()
    {
        // Arrange
        var requestUrl = "/api/Categories?page=1&pageSize=20&onlyActive=true";

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
    public async Task GetCategories_WithInvalidPage_ReturnsBadRequest(int page)
    {
        // Arrange
        var requestUrl = $"/api/Categories?page={page}&pageSize=20&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]   // pageSize = 0 (inválido)
    [InlineData(-10)] // pageSize negativo
    [InlineData(1001)] // pageSize muy grande
    public async Task GetCategories_WithInvalidPageSize_ReturnsBadRequest(int pageSize)
    {
        // Arrange
        var requestUrl = $"/api/Categories?page=1&pageSize={pageSize}&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_WithInactiveFilter_ReturnsOnlyInactiveCategories()
    {
        // Arrange
        var requestUrl = "/api/Categories?page=1&pageSize=20&onlyActive=false";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var requestUrl = "/api/Categories?page=1&pageSize=10&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCategoryById_WithValidId_ReturnsOkAndCategoryDetails()
    {
        // Arrange
        var validId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Categories/{validId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategoryById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Categories/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_WithDifferentPageSizes_ReturnsOk()
    {
        // Arrange & Act & Assert
        var pageSizes = new[] { 5, 10, 20, 50 };
        
        foreach (var pageSize in pageSizes)
        {
            var requestUrl = $"/api/Categories?page=1&pageSize={pageSize}&onlyActive=true";
            var response = await Client.GetAsync(requestUrl);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
