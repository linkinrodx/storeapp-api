using StoreApp.Domain.DTOs.Responses;
using System.Linq.Expressions;

namespace StoreApp.Domain.Services.Interfaces;

/// <summary>
/// Generic service interface for CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type that implements the service.</typeparam>
public interface IBaseService<T> where T : class
{
    /// <summary>
    /// Retrieves a paged list of entities.
    /// </summary>
    Task<PagedResponse<T>> GetAllAsync(int page, int pageSize, bool onlyActive);

    /// <summary>
    /// Retrieves a single entity by its ID.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task<T?> UpdateAsync(Guid id, T entity);

    /// <summary>
    /// Performs a soft delete of an entity.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if an entity exists by its ID.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}