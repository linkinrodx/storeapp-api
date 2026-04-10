using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreApp.Api.Constants;
using StoreApp.Api.Data;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace StoreApp.Api.Services.Implementations;

public class OrderService : BaseService<Order>, IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(IRepository<Order> repository, IMapper mapper, AppDbContext context) : base(repository, mapper)
    {
        _context = context;
    }

    protected override Expression<Func<Order, bool>> GetIdExpression(Guid id)
    {
        return x => x.Id == id;
    }

    public override async Task<PagedResponse<Order>> GetAllAsync(int page, int pageSize, bool onlyActive)
    {
        return await GetAllAsync(page, pageSize, null, null);
    }

    public async Task<PagedResponse<Order>> GetAllAsync(int page, int pageSize, Guid? userId, string? status)
    {
        var query = _context.Orders.Include(x => x.Items).AsQueryable();
        if (userId.HasValue) query = query.Where(x => x.UserId == userId);
        if (status is not null) query = query.Where(x => x.Status == status);
        
        var total = await query.CountAsync();
        var data = await query.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        return new PagedResponse<Order>(data, total, page, pageSize);
    }

    public override async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Order> CreateAsync(Guid userId, Order order)
    {
        order.UserId = userId;
        order.CreatedAt = DateTimeOffset.UtcNow;
        order.UpdatedAt = DateTimeOffset.UtcNow;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateStatusAsync(Guid id, string status)
    {
        if (!OrderStatus.IsValid(status)) return null;

        var entity = await _context.Orders.FindAsync(id);
        if (entity is null) return null;

        entity.Status = status;
        if (status == OrderStatus.Shipped) entity.ShippedAt = DateTimeOffset.UtcNow;
        if (status == OrderStatus.Delivered) entity.DeliveredAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var entity = await _context.Orders.FindAsync(id);
        if (entity is null) return false;
        if (entity.Status == OrderStatus.Shipped || entity.Status == OrderStatus.Delivered) return false;

        entity.Status = OrderStatus.Cancelled;
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}