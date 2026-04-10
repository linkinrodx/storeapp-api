using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface IBrandService : IBaseService<Brand>
{
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
}