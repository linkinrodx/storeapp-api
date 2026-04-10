using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApp.Api.Data;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class WishlistService : BaseService<Wishlist>, IWishlistService
{
    private readonly AppDbContext _context;

    public WishlistService(IRepository<Wishlist> repository, IMapper mapper, AppDbContext context) : base(repository, mapper)
    {
        _context = context;
    }

    protected override Expression<Func<Wishlist, bool>> GetIdExpression(Guid id)
    {
        return x => x.UserId == id;
    }

    public async Task<IEnumerable<Wishlist>> GetByUserAsync(Guid userId)
    {
        return await _context.Wishlists
            .Include(x => x.Product)
            .Where(x => x.UserId == userId && x.IsActive == true)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Wishlist> AddAsync(Guid userId, Guid productId)
    {
        var existing = await _context.Wishlists.FindAsync(userId, productId);
        if (existing is not null)
        {
            existing.IsActive = true;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        var item = new Wishlist
        {
            UserId = userId,
            ProductId = productId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        _context.Wishlists.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid productId)
    {
        var item = await _context.Wishlists.FindAsync(userId, productId);
        if (item is null) return false;
        _context.Wishlists.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}