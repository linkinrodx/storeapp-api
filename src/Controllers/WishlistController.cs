using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/wishlist")]
[Produces("application/json")]
public class WishlistController(IWishlistService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistResponse>>> GetWishlist(Guid userId)
    {
        var result = await service.GetByUserAsync(userId);
        return Ok(mapper.Map<IEnumerable<WishlistResponse>>(result));
    }

    [HttpPost("{productId:guid}")]
    public async Task<ActionResult<WishlistResponse>> AddToWishlist(Guid userId, Guid productId)
    {
        var result = await service.AddAsync(userId, productId);
        return Ok(mapper.Map<WishlistResponse>(result));
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid userId, Guid productId)
    {
        var deleted = await service.RemoveAsync(userId, productId);
        return deleted ? NoContent() : NotFound();
    }
}