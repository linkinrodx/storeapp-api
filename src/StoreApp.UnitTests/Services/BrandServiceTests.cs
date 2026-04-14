using AutoMapper;
using Moq;
using StoreApp.Domain.Models;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;

namespace StoreApp.Test.Services;

public class BrandServiceTests
{
    private readonly Mock<IRepository<Brand>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BrandService _service;

    public BrandServiceTests()
    {
        _mockRepository = new Mock<IRepository<Brand>>();
        _mockMapper = new Mock<IMapper>();
        _service = new BrandService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResponse()
    {
        var brands = new List<Brand>
        {
            new() { Id = Guid.NewGuid(), Name = "Brand1", Slug = "brand1" },
            new() { Id = Guid.NewGuid(), Name = "Brand2", Slug = "brand2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(1, 20, true)).ReturnsAsync(brands);
        _mockRepository.Setup(r => r.CountAsync(true)).ReturnsAsync(2);

        var result = await _service.GetAllAsync(1, 20, true);

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "TestBrand", Slug = "test-brand" };
        _mockRepository.Setup(r => r.GetByIdAsync(brandId)).ReturnsAsync(brand);

        var result = await _service.GetByIdAsync(brandId);

        Assert.NotNull(result);
        Assert.Equal(brandId, result.Id);
        Assert.Equal("TestBrand", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var brandId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(brandId)).ReturnsAsync((Brand?)null);

        var result = await _service.GetByIdAsync(brandId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsEntity()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "NewBrand", Slug = "new-brand" };
        _mockRepository.Setup(r => r.CreateAsync(brand)).ReturnsAsync(brand);

        var result = await _service.CreateAsync(brand);

        Assert.NotNull(result);
        Assert.Equal("NewBrand", result.Name);
        _mockRepository.Verify(r => r.CreateAsync(brand), Times.Once);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsTrue_WhenSlugIsNew()
    {
        var slug = "new-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>()))
            .ReturnsAsync(false);

        var result = await _service.IsSlugUniqueAsync(slug);

        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsFalse_WhenSlugExists()
    {
        var slug = "existing-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>()))
            .ReturnsAsync(true);

        var result = await _service.IsSlugUniqueAsync(slug);

        Assert.False(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsTrue_WhenSameBrand()
    {
        var brandId = Guid.NewGuid();
        var slug = "my-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>()))
            .ReturnsAsync(false);

        var result = await _service.IsSlugUniqueAsync(slug, brandId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenSuccessful()
    {
        var brandId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(brandId)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(brandId);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "UpdatedBrand", Slug = "updated-brand" };
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateAsync(brand)).ReturnsAsync(brand);

        var result = await _service.UpdateAsync(brandId, brand);

        Assert.NotNull(result);
        Assert.Equal("UpdatedBrand", result.Name);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
    {
        var brandId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>())).ReturnsAsync(true);

        var result = await _service.ExistsAsync(brandId);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenEntityNotExists()
    {
        var brandId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>())).ReturnsAsync(false);

        var result = await _service.ExistsAsync(brandId);

        Assert.False(result);
    }
}