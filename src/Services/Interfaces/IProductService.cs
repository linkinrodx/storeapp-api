using StoreApp.Api.DTOs.Requests;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface IProductService : IBaseService<Product>
{
    new Task<PagedResponse<Product>> GetAllAsync(int page, int pageSize, bool onlyActive, Guid? brandId, Guid? familyId, Guid? categoryId, string? search);
    new Task<Product?> GetByIdAsync(Guid id);
}