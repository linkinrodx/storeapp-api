using AutoMapper;
using StoreApp.Domain.Models;
using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;
using StoreApp.Domain.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Domain.Services.Implementations;

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