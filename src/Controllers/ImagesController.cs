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
public class ImagesController(IImageService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByEntity(
        [FromQuery] Guid entityId,
        [FromQuery] string? entityType = null)
    {
        var result = await service.GetByEntityAsync(entityId, entityType);
        return Ok(mapper.Map<IEnumerable<ImageResponse>>(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImageResponse>> GetById(Guid id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<ImageResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ImageResponse>> Create([FromBody] CreateImageRequest req)
    {
        if (!ImageEntityTypes.IsValid(req.EntityType))
            return BadRequest(new { message = string.Format(ErrorMessages.InvalidEntityType, string.Join(", ", ImageEntityTypes.All)) });

        var entity = new Image
        {
            Id = Guid.NewGuid(),
            EntityId = req.EntityId,
            EntityType = req.EntityType,
            Url = req.Url,
            AltText = req.AltText,
            SortOrder = req.SortOrder ?? 0,
            IsPrimary = req.IsPrimary ?? false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var created = await service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<ImageResponse>(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ImageResponse>> Update(Guid id, [FromBody] UpdateImageRequest req)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (req.Url is not null) existing.Url = req.Url;
        if (req.AltText is not null) existing.AltText = req.AltText;
        if (req.SortOrder is not null) existing.SortOrder = req.SortOrder;
        if (req.IsPrimary is not null) existing.IsPrimary = req.IsPrimary;
        if (req.IsActive is not null) existing.IsActive = req.IsActive;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await service.UpdateAsync(id, existing);
        return Ok(mapper.Map<ImageResponse>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}