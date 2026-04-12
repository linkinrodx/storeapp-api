using AutoMapper;
using Moq;
using StoreApp.Domain.Models;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;
using StoreApp.Domain.Services.Interfaces;
using Xunit;

namespace StoreApp.Test.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Category>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<IRepository<Category>>();
        _mockMapper = new Mock<IMapper>();
        _service = new CategoryService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResponse()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Label = "Category1", Slug = "category1" },
            new() { Id = Guid.NewGuid(), Label = "Category2", Slug = "category2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(1, 20, true)).ReturnsAsync(categories);
        _mockRepository.Setup(r => r.CountAsync(true)).ReturnsAsync(2);

        var result = await _service.GetAllAsync(1, 20, true);

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Label = "TestCategory", Slug = "test-category" };
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

        var result = await _service.GetByIdAsync(categoryId);

        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("TestCategory", result.Label);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var categoryId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        var result = await _service.GetByIdAsync(categoryId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsEntity()
    {
        var category = new Category { Id = Guid.NewGuid(), Label = "NewCategory", Slug = "new-category" };
        _mockRepository.Setup(r => r.CreateAsync(category)).ReturnsAsync(category);

        var result = await _service.CreateAsync(category);

        Assert.NotNull(result);
        Assert.Equal("NewCategory", result.Label);
        _mockRepository.Verify(r => r.CreateAsync(category), Times.Once);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsTrue_WhenSlugIsNew()
    {
        var slug = "new-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
            .ReturnsAsync(false);

        var result = await _service.IsSlugUniqueAsync(slug);

        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsFalse_WhenSlugExists()
    {
        var slug = "existing-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
            .ReturnsAsync(true);

        var result = await _service.IsSlugUniqueAsync(slug);

        Assert.False(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsTrue_WhenSameCategory()
    {
        var categoryId = Guid.NewGuid();
        var slug = "my-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
            .ReturnsAsync(false);

        var result = await _service.IsSlugUniqueAsync(slug, categoryId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenSuccessful()
    {
        var categoryId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(categoryId)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(categoryId);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Label = "UpdatedCategory", Slug = "updated-category" };
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateAsync(category)).ReturnsAsync(category);

        var result = await _service.UpdateAsync(categoryId, category);

        Assert.NotNull(result);
        Assert.Equal("UpdatedCategory", result.Label);
    }
}