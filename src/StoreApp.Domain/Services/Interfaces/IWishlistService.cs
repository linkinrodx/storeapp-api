using StoreApp.Domain.Models;

namespace StoreApp.Domain.Services.Interfaces;

/// <summary>
/// Service interface for Wishlist entity operations.
/// </summary>
public interface IWishlistService : IBaseService<Wishlist>
{
    /// <summary>
    /// Retrieves all active wishlist items for a user.
    /// </summary>
    Task<IEnumerable<Wishlist>> GetByUserAsync(Guid userId);

    /// <summary>
    /// Adds a product to user's wishlist.
    /// </summary>
    Task<Wishlist> AddAsync(Guid userId, Guid productId);

    /// <summary>
    /// Removes a product from user's wishlist.
    /// </summary>
    Task<bool> RemoveAsync(Guid userId, Guid productId);
}