using StoreApp.Api.Models;

namespace StoreApp.Api.Services.Interfaces;

/// <summary>
/// Service interface for Category entity operations.
/// </summary>
public interface ICategoryService : IBaseService<Category>
{
    /// <summary>
    /// Checks if a slug is unique (not used by other categories).
    /// </summary>
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
}