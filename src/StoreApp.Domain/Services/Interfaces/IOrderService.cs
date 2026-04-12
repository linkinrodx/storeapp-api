using StoreApp.Domain.DTOs.Responses;
using StoreApp.Domain.Models;

namespace StoreApp.Domain.Services.Interfaces;

/// <summary>
/// Service interface for Order entity operations.
/// </summary>
public interface IOrderService : IBaseService<Order>
{
    Task<PagedResponse<Order>> GetAllAsync(int page, int pageSize, Guid? userId, string? status);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Guid userId, Order order);
    Task<Order?> UpdateStatusAsync(Guid id, string status);
    Task<bool> CancelAsync(Guid id);
}