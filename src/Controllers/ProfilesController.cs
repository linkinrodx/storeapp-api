using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Api.DTOs.Requests;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProfilesController(IProfileService service, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfileResponse>> GetById(Guid id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<ProfileResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProfileResponse>> Update(Guid id, [FromBody] UpdateProfileRequest req)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (req.FullName is not null) existing.FullName = req.FullName;
        if (req.Phone is not null) existing.Phone = req.Phone;
        if (req.ShippingAddress is not null) existing.ShippingAddress = req.ShippingAddress;
        if (req.ShippingCity is not null) existing.ShippingCity = req.ShippingCity;
        if (req.ShippingReference is not null) existing.ShippingReference = req.ShippingReference;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await service.UpdateAsync(id, existing);
        return Ok(mapper.Map<ProfileResponse>(updated));
    }
}