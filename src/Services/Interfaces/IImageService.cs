using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface IImageService : IBaseService<Image>
{
    Task<IEnumerable<Image>> GetByEntityAsync(Guid entityId, string? entityType = null);
}