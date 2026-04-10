using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface ICategoryService : IBaseService<Category>
{
    Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null);
}