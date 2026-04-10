using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Data;
using StoreApp.Domain.Repositories.Interfaces;

namespace StoreApp.Domain.Repositories.Implementations;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = 20, bool onlyActive = true)
    {
        var query = _context.Set<T>().AsQueryable();
        
        var property = typeof(T).GetProperty("IsActive");
        if (property != null && onlyActive)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var constant = Expression.Constant(true);
            var comparison = Expression.Equal(propertyAccess, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
            query = query.Where(lambda);
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<int> CountAsync(bool onlyActive = true)
    {
        var query = _context.Set<T>().AsQueryable();
        
        var property = typeof(T).GetProperty("IsActive");
        if (property != null && onlyActive)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var constant = Expression.Constant(true);
            var comparison = Expression.Equal(propertyAccess, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
            query = query.Where(lambda);
        }

        return await query.CountAsync();
    }

    public virtual async Task<T?> GetByIdAsync(object id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(object id, T entity)
    {
        var existing = await _context.Set<T>().FindAsync(id);
        if (existing is null) throw new KeyNotFoundException($"Entity with id {id} not found");
        
        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(object id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity is null) return false;

        var property = typeof(T).GetProperty("IsActive");
        if (property != null)
        {
            property.SetValue(entity, false);
            _context.Set<T>().Update(entity);
        }
        else
        {
            _context.Set<T>().Remove(entity);
        }
        
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }
}