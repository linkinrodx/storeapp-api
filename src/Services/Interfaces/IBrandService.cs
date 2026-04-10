using StoreApp.Api.Models;

namespace StoreApp.Api.Services.Interfaces;

/// <summary>
/// Service interface for Brand entity operations.
/// </summary>
public interface IBrandService : IBaseService<Brand>
{
    /// <summary>
    /// Checks if a slug is unique (not used by other brands).
    /// </summary>
    /// <param name="slug">The slug to check.</param>
    /// <param name="excludeId">Optional brand ID to exclude from the check (for updates).</param>
    /// <returns>True if the slug is unique.</returns>
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
}