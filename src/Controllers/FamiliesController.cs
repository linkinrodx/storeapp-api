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
public class FamiliesController(IFamilyService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<FamilyResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlyActive = true)
    {
        var result = await service.GetAllAsync(page, pageSize, onlyActive);
        return Ok(mapper.Map<PagedResponse<FamilyResponse>>(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FamilyResponse>> GetById(Guid id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<FamilyResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<FamilyResponse>> Create([FromBody] CreateFamilyRequest req)
    {
        if (!await service.IsSlugUniqueAsync(req.Slug))
            return Conflict(new { message = ErrorMessages.SlugAlreadyInUse });

        var entity = new Family
        {
            Id = Guid.NewGuid(),
            Label = req.Label,
            Slug = req.Slug,
            SortOrder = req.SortOrder ?? 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var created = await service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<FamilyResponse>(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FamilyResponse>> Update(Guid id, [FromBody] UpdateFamilyRequest req)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (req.Slug is not null && req.Slug != existing.Slug && !await service.IsSlugUniqueAsync(req.Slug, id))
            return Conflict(new { message = ErrorMessages.SlugAlreadyInUse });

        if (req.Label is not null) existing.Label = req.Label;
        if (req.Slug is not null) existing.Slug = req.Slug;
        if (req.SortOrder is not null) existing.SortOrder = req.SortOrder;
        if (req.IsActive is not null) existing.IsActive = req.IsActive;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await service.UpdateAsync(id, existing);
        return Ok(mapper.Map<FamilyResponse>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}