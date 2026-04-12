using StoreApp.Domain.Models;

namespace StoreApp.Domain.Services.Interfaces;

/// <summary>
/// Service interface for Image entity operations.
/// </summary>
public interface IImageService : IBaseService<Image>
{
    /// <summary>
    /// Retrieves images by entity ID and optionally by entity type.
    /// </summary>
    Task<IEnumerable<Image>> GetByEntityAsync(Guid entityId, string? entityType = null);
}