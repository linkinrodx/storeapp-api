using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Api.DTOs.Requests;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/cart")]
[Produces("application/json")]
public class CartController(ICartService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemResponse>>> GetCart(Guid userId)
    {
        var result = await service.GetByUserAsync(userId);
        return Ok(mapper.Map<IEnumerable<CartItemResponse>>(result));
    }

    [HttpPost]
    public async Task<ActionResult<CartItemResponse>> UpsertItem(Guid userId, [FromBody] UpsertCartItemRequest req)
    {
        var result = await service.UpsertItemAsync(userId, req.ProductId, req.Quantity);
        return Ok(mapper.Map<CartItemResponse>(result));
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid userId, Guid productId)
    {
        var deleted = await service.RemoveItemAsync(userId, productId);
        return deleted ? NoContent() : NotFound();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        await service.ClearCartAsync(userId);
        return NoContent();
    }
}