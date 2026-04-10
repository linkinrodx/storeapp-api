using AutoMapper;
using StoreApp.Api.Constants;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class BrandService : BaseService<Brand>, IBrandService
{
    public BrandService(IRepository<Brand> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Expression<Func<Brand, bool>> GetIdExpression(Guid id)
    {
        return x => x.Id == id;
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null)
    {
        var query = _repository.ExistsAsync(x => x.Slug == slug);
        if (excludeId.HasValue)
        {
            query = _repository.ExistsAsync(x => x.Slug == slug && x.Id != excludeId);
        }
        return !await query;
    }
}