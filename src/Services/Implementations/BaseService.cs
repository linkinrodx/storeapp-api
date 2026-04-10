using AutoMapper;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Implementations;

public abstract class BaseService<T> : IBaseService<T> where T : class
{
    protected readonly IRepository<T> _repository;
    protected readonly IMapper _mapper;

    protected BaseService(IRepository<T> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<PagedResponse<T>> GetAllAsync(int page, int pageSize, bool onlyActive)
    {
        var data = await _repository.GetAllAsync(page, pageSize, onlyActive);
        var total = await _repository.CountAsync(onlyActive);
        return new PagedResponse<T>(data, total, page, pageSize);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        return await _repository.CreateAsync(entity);
    }

    public virtual async Task<T?> UpdateAsync(Guid id, T entity)
    {
        var exists = await ExistsAsync(id);
        if (!exists) return null;

        await _repository.UpdateAsync(entity);
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(GetIdExpression(id));
    }

    protected abstract System.Linq.Expressions.Expression<Func<T, bool>> GetIdExpression(Guid id);
}