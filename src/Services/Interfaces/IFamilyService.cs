using StoreApp.Api.Models;

namespace StoreApp.Api.Services.Interfaces;

/// <summary>
/// Service interface for Family entity operations.
/// </summary>
public interface IFamilyService : IBaseService<Family>
{
    /// <summary>
    /// Checks if a slug is unique (not used by other families).
    /// </summary>
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
}