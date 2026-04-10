using StoreApp.Domain.Models;

namespace StoreApp.Domain.Services.Interfaces;

/// <summary>
/// Service interface for CartItem entity operations.
/// </summary>
public interface ICartService : IBaseService<CartItem>
{
    /// <summary>
    /// Retrieves all active cart items for a user.
    /// </summary>
    Task<IEnumerable<CartItem>> GetByUserAsync(Guid userId);

    /// <summary>
    /// Adds or updates a cart item (upsert operation).
    /// </summary>
    Task<CartItem> UpsertItemAsync(Guid userId, Guid productId, int quantity);

    /// <summary>
    /// Removes a cart item.
    /// </summary>
    Task<bool> RemoveItemAsync(Guid userId, Guid productId);

    /// <summary>
    /// Clears all items from user's cart.
    /// </summary>
    Task<bool> ClearCartAsync(Guid userId);
}