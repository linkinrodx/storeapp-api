using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface IWishlistService : IBaseService<Wishlist>
{
    Task<IEnumerable<Wishlist>> GetByUserAsync(Guid userId);
    Task<Wishlist> AddAsync(Guid userId, Guid productId);
    Task<bool> RemoveAsync(Guid userId, Guid productId);
}