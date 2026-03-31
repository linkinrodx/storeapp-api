using System.Linq.Expressions;

namespace StoreApp.Api.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = 20, bool onlyActive = true);
    Task<int> CountAsync(bool onlyActive = true);
    Task<T?> GetByIdAsync(object id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(object id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}
