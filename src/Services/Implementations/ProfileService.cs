using AutoMapper;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class ProfileService : BaseService<UserProfile>, IProfileService
{
    public ProfileService(IRepository<UserProfile> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Expression<Func<UserProfile, bool>> GetIdExpression(Guid id)
    {
        return x => x.Id == id;
    }
}