using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApp.Api.Data;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class ProductService : BaseService<Product>, IProductService
{
    private readonly AppDbContext _context;

    public ProductService(IRepository<Product> repository, IMapper mapper, AppDbContext context) : base(repository, mapper)
    {
        _context = context;
    }

    protected override Expression<Func<Product, bool>> GetIdExpression(Guid id)
    {
        return x => x.Id == id;
    }

    public override async Task<PagedResponse<Product>> GetAllAsync(int page, int pageSize, bool onlyActive)
    {
        return await GetAllAsync(page, pageSize, onlyActive, null, null, null, null);
    }

    public async Task<PagedResponse<Product>> GetAllAsync(int page, int pageSize, bool onlyActive, Guid? brandId, Guid? familyId, Guid? categoryId, string? search)
    {
        var query = _context.Products
            .Include(x => x.Brand)
            .Include(x => x.Family)
            .Include(x => x.Category)
            .AsQueryable();

        if (onlyActive) query = query.Where(x => x.IsActive == true);
        if (brandId.HasValue) query = query.Where(x => x.BrandId == brandId);
        if (familyId.HasValue) query = query.Where(x => x.FamilyId == familyId);
        if (categoryId.HasValue) query = query.Where(x => x.CategoryId == categoryId);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));

        var total = await query.CountAsync();
        var data = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        return new PagedResponse<Product>(data, total, page, pageSize);
    }

    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(x => x.Brand)
            .Include(x => x.Family)
            .Include(x => x.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}