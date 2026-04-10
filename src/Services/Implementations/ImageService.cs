using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApp.Api.Data;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class ImageService : BaseService<Image>, IImageService
{
    private readonly AppDbContext _context;

    public ImageService(IRepository<Image> repository, IMapper mapper, AppDbContext context) : base(repository, mapper)
    {
        _context = context;
    }

    protected override Expression<Func<Image, bool>> GetIdExpression(Guid id)
    {
        return x => x.Id == id;
    }

    public async Task<IEnumerable<Image>> GetByEntityAsync(Guid entityId, string? entityType = null)
    {
        var query = _context.Images.Where(x => x.EntityId == entityId && x.IsActive == true);
        if (entityType is not null) query = query.Where(x => x.EntityType == entityType);
        return await query.OrderBy(x => x.SortOrder).AsNoTracking().ToListAsync();
    }
}