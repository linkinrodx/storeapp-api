using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface ICartService : IBaseService<CartItem>
{
    Task<IEnumerable<CartItem>> GetByUserAsync(Guid userId);
    Task<CartItem> UpsertItemAsync(Guid userId, Guid productId, int quantity);
    Task<bool> RemoveItemAsync(Guid userId, Guid productId);
    Task<bool> ClearCartAsync(Guid userId);
}