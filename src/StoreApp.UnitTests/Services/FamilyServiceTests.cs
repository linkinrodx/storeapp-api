using AutoMapper;
using Moq;
using StoreApp.Domain.Models;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;

namespace StoreApp.Test.Services;

public class FamilyServiceTests
{
    private readonly Mock<IRepository<Family>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly FamilyService _service;

    public FamilyServiceTests()
    {
        _mockRepository = new Mock<IRepository<Family>>();
        _mockMapper = new Mock<IMapper>();
        _service = new FamilyService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResponse()
    {
        var families = new List<Family>
        {
            new() { Id = Guid.NewGuid(), Label = "Family1", Slug = "family1" },
            new() { Id = Guid.NewGuid(), Label = "Family2", Slug = "family2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(1, 20, true)).ReturnsAsync(families);
        _mockRepository.Setup(r => r.CountAsync(true)).ReturnsAsync(2);

        var result = await _service.GetAllAsync(1, 20, true);

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        var familyId = Guid.NewGuid();
        var family = new Family { Id = familyId, Label = "TestFamily", Slug = "test-family" };
        _mockRepository.Setup(r => r.GetByIdAsync(familyId)).ReturnsAsync(family);

        var result = await _service.GetByIdAsync(familyId);

        Assert.NotNull(result);
        Assert.Equal(familyId, result.Id);
        Assert.Equal("TestFamily", result.Label);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var familyId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(familyId)).ReturnsAsync((Family?)null);

        var result = await _service.GetByIdAsync(familyId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsEntity()
    {
        var family = new Family { Id = Guid.NewGuid(), Label = "NewFamily", Slug = "new-family" };
        _mockRepository.Setup(r => r.CreateAsync(family)).ReturnsAsync(family);

        var result = await _service.CreateAsync(family);

        Assert.NotNull(result);
        Assert.Equal("NewFamily", result.Label);
        _mockRepository.Verify(r => r.CreateAsync(family), Times.Once);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsTrue_WhenSlugIsNew()
    {
        var slug = "new-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Family, bool>>>()))
            .ReturnsAsync(false);

        var result = await _service.IsSlugUniqueAsync(slug);

        Assert.True(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsFalse_WhenSlugExists()
    {
        var slug = "existing-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Family, bool>>>()))
            .ReturnsAsync(true);

        var result = await _service.IsSlugUniqueAsync(slug);

        Assert.False(result);
    }

    [Fact]
    public async Task IsSlugUniqueAsync_ReturnsTrue_WhenSameFamily()
    {
        var familyId = Guid.NewGuid();
        var slug = "my-slug";
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Family, bool>>>()))
            .ReturnsAsync(false);

        var result = await _service.IsSlugUniqueAsync(slug, familyId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenSuccessful()
    {
        var familyId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(familyId)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(familyId);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        var familyId = Guid.NewGuid();
        var family = new Family { Id = familyId, Label = "UpdatedFamily", Slug = "updated-family" };
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Family, bool>>>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateAsync(family)).ReturnsAsync(family);

        var result = await _service.UpdateAsync(familyId, family);

        Assert.NotNull(result);
        Assert.Equal("UpdatedFamily", result.Label);
    }
}