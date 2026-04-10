using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Api.Constants;
using StoreApp.Api.DTOs.Requests;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(IOrderService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? status = null)
    {
        var result = await service.GetAllAsync(page, pageSize, userId, status);
        return Ok(mapper.Map<PagedResponse<OrderResponse>>(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<OrderResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromQuery] Guid userId, [FromBody] CreateOrderRequest req)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Subtotal = req.Subtotal,
            ShippingCost = req.ShippingCost ?? 0,
            Tax = req.Tax ?? 0,
            Total = req.Total,
            ShippingAddress = req.ShippingAddress,
            ShippingCity = req.ShippingCity,
            ShippingReference = req.ShippingReference,
            ShippingMethod = req.ShippingMethod,
            PaymentMethod = req.PaymentMethod,
            PaymentReference = req.PaymentReference,
            Items = req.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                BrandName = i.BrandName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            }).ToList()
        };
        var created = await service.CreateAsync(userId, order);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<OrderResponse>(created));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest req)
    {
        if (!OrderStatus.IsValid(req.Status))
            return BadRequest(new { message = string.Format(ErrorMessages.InvalidOrderStatus, string.Join(", ", OrderStatus.All)) });

        var updated = await service.UpdateStatusAsync(id, req.Status);
        return updated is null ? NotFound() : Ok(mapper.Map<OrderResponse>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var cancelled = await service.CancelAsync(id);
        if (!cancelled) return BadRequest(new { message = ErrorMessages.CannotCancelShippedOrDelivered });
        return NoContent();
    }
}