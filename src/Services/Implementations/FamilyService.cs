using AutoMapper;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class FamilyService : BaseService<Family>, IFamilyService
{
    public FamilyService(IRepository<Family> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Expression<Func<Family, bool>> GetIdExpression(Guid id)
    {
        return x => x.Id == id;
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return !await _repository.ExistsAsync(x => x.Slug == slug && x.Id != excludeId.Value);
        }
        return !await _repository.ExistsAsync(x => x.Slug == slug);
    }
}