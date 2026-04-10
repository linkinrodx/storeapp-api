using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApp.Api.Data;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class CartService : BaseService<CartItem>, ICartService
{
    private readonly AppDbContext _context;

    public CartService(IRepository<CartItem> repository, IMapper mapper, AppDbContext context) : base(repository, mapper)
    {
        _context = context;
    }

    protected override Expression<Func<CartItem, bool>> GetIdExpression(Guid id)
    {
        return x => x.UserId == id;
    }

    public async Task<IEnumerable<CartItem>> GetByUserAsync(Guid userId)
    {
        return await _context.CartItems
            .Include(x => x.Product)
            .Where(x => x.UserId == userId && x.IsActive == true)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CartItem> UpsertItemAsync(Guid userId, Guid productId, int quantity)
    {
        var existing = await _context.CartItems.FindAsync(userId, productId);
        if (existing is not null)
        {
            existing.Quantity = quantity;
            existing.IsActive = true;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            var item = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            _context.CartItems.Add(item);
            existing = item;
        }
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> RemoveItemAsync(Guid userId, Guid productId)
    {
        var item = await _context.CartItems.FindAsync(userId, productId);
        if (item is null) return false;
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(Guid userId)
    {
        var items = _context.CartItems.Where(x => x.UserId == userId);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
        return true;
    }
}