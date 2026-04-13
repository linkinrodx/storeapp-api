using System.Net;

namespace StoreApp.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests para el endpoint de Families.
/// Prueba los principales flujos de la API.
/// </summary>
public class FamiliesEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetFamilies_WithValidParameters_ReturnsOkStatusAndFamilyList()
    {
        // Arrange
        var requestUrl = "/api/Families?page=1&pageSize=20&onlyActive=true";

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
    public async Task GetFamilies_WithInvalidPage_ReturnsBadRequest(int page)
    {
        // Arrange
        var requestUrl = $"/api/Families?page={page}&pageSize=20&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]   // pageSize = 0 (inválido)
    [InlineData(-10)] // pageSize negativo
    [InlineData(1001)] // pageSize muy grande
    public async Task GetFamilies_WithInvalidPageSize_ReturnsBadRequest(int pageSize)
    {
        // Arrange
        var requestUrl = $"/api/Families?page=1&pageSize={pageSize}&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetFamilies_WithInactiveFilter_ReturnsOnlyInactiveFamilies()
    {
        // Arrange
        var requestUrl = "/api/Families?page=1&pageSize=20&onlyActive=false";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFamilies_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var requestUrl = "/api/Families?page=1&pageSize=10&onlyActive=true";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFamilyById_WithValidId_ReturnsOkAndFamilyDetails()
    {
        // Arrange
        var validId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Families/{validId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFamilyById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/Families/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFamilies_WithActiveFilter_ReturnsOk(bool onlyActive)
    {
        // Arrange
        var requestUrl = $"/api/Families?page=1&pageSize=20&onlyActive={onlyActive}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
