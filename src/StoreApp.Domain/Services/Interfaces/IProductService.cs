using StoreApp.Domain.DTOs.Responses;
using StoreApp.Domain.Models;

namespace StoreApp.Domain.Services.Interfaces;

/// <summary>
/// Service interface for Product entity operations.
/// </summary>
public interface IProductService : IBaseService<Product>
{
    Task<PagedResponse<Product>> GetAllAsync(int page, int pageSize, bool onlyActive, Guid? brandId, Guid? familyId, Guid? categoryId, string? search);
    Task<Product?> GetByIdAsync(Guid id);
}