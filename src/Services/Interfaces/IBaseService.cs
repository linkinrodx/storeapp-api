using StoreApp.Api.DTOs.Responses;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Interfaces;

public interface IBaseService<T> where T : class
{
    Task<PagedResponse<T>> GetAllAsync(int page, int pageSize, bool onlyActive);
    Task<T?> GetByIdAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(Guid id, T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}