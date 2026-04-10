using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Services.Interfaces;

public interface IOrderService : IBaseService<Order>
{
    new Task<PagedResponse<Order>> GetAllAsync(int page, int pageSize, Guid? userId, string? status);
    new Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Guid userId, Order order);
    Task<Order?> UpdateStatusAsync(Guid id, string status);
    Task<bool> CancelAsync(Guid id);
}