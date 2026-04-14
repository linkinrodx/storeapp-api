using AutoMapper;
using Moq;
using StoreApp.Domain.Models;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;
using StoreApp.Domain.Services.Interfaces;

namespace StoreApp.Test.Services;

public class ProductServiceTests
{
    private Mock<IRepository<Product>> CreateMockRepo()
    {
        return new Mock<IRepository<Product>>();
    }

    private TestProductService CreateService(Mock<IRepository<Product>> mockRepo)
    {
        var mockMapper = new Mock<IMapper>();
        return new TestProductService(mockRepo.Object, mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_AddsProduct()
    {
        var mockRepo = CreateMockRepo();
        var product = new Product { Id = Guid.NewGuid(), Name = "NewProduct", Price = 250 };
        mockRepo.Setup(r => r.CreateAsync(product)).ReturnsAsync(product);

        var service = CreateService(mockRepo);
        var result = await service.CreateAsync(product);

        Assert.NotNull(result);
        Assert.Equal("NewProduct", result.Name);
        mockRepo.Verify(r => r.CreateAsync(product), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResponse()
    {
        var mockRepo = CreateMockRepo();
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "P1", Price = 100 },
            new() { Id = Guid.NewGuid(), Name = "P2", Price = 200 }
        };
        mockRepo.Setup(r => r.GetAllAsync(1, 10, true)).ReturnsAsync(products);
        mockRepo.Setup(r => r.CountAsync(true)).ReturnsAsync(2);

        var service = CreateService(mockRepo);
        var result = await service.GetAllAsync(1, 10, true);

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoData()
    {
        var mockRepo = CreateMockRepo();
        var products = new List<Product>();
        mockRepo.Setup(r => r.GetAllAsync(1, 10, true)).ReturnsAsync(products);
        mockRepo.Setup(r => r.CountAsync(true)).ReturnsAsync(0);

        var service = CreateService(mockRepo);
        var result = await service.GetAllAsync(1, 10, true);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "TestProduct", Price = 100 };
        mockRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var service = CreateService(mockRepo);
        var result = await service.GetByIdAsync(productId);

        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("TestProduct", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        mockRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var service = CreateService(mockRepo);
        var result = await service.GetByIdAsync(productId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity_WhenExists()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "UpdatedProduct", Price = 300 };
        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(true);
        mockRepo.Setup(r => r.UpdateAsync(product)).ReturnsAsync(product);

        var service = CreateService(mockRepo);
        var result = await service.UpdateAsync(productId, product);

        Assert.NotNull(result);
        Assert.Equal("UpdatedProduct", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotExists()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "UpdatedProduct" };
        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(false);

        var service = CreateService(mockRepo);
        var result = await service.UpdateAsync(productId, product);

        Assert.Null(result);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenSuccessful()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        mockRepo.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(true);

        var service = CreateService(mockRepo);
        var result = await service.DeleteAsync(productId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        mockRepo.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(false);

        var service = CreateService(mockRepo);
        var result = await service.DeleteAsync(productId);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(true);

        var service = CreateService(mockRepo);
        var result = await service.ExistsAsync(productId);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenEntityNotExists()
    {
        var mockRepo = CreateMockRepo();
        var productId = Guid.NewGuid();
        mockRepo.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>())).ReturnsAsync(false);

        var service = CreateService(mockRepo);
        var result = await service.ExistsAsync(productId);

        Assert.False(result);
    }
}

public class TestProductService : BaseService<Product>, IBaseService<Product>
{
    public TestProductService(IRepository<Product> repository, IMapper mapper) : base(repository, mapper) { }

    protected override System.Linq.Expressions.Expression<Func<Product, bool>> GetIdExpression(Guid id)
        => x => x.Id == id;
}