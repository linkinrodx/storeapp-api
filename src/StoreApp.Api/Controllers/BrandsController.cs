using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StoreApp.Domain.Constants;
using StoreApp.Domain.Controllers.Base;
using StoreApp.Domain.DTOs.Requests;
using StoreApp.Domain.DTOs.Responses;
using StoreApp.Domain.Models;
using StoreApp.Domain.Services.Interfaces;

namespace StoreApp.Domain.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController(IBrandService service, IMapper mapper) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<BrandResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlyActive = true)
    {
        var result = await service.GetAllAsync(page, pageSize, onlyActive);
        return Ok(mapper.Map<PagedResponse<BrandResponse>>(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BrandResponse>> GetById(Guid id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<BrandResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<BrandResponse>> Create([FromBody] CreateBrandRequest req)
    {
        if (!await service.IsSlugUniqueAsync(req.Slug))
            return Conflict(new { message = ErrorMessages.SlugAlreadyInUse });

        var entity = new Brand
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Slug = req.Slug,
            Description = req.Description,
            IsFeatured = req.IsFeatured ?? false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var created = await service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<BrandResponse>(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BrandResponse>> Update(Guid id, [FromBody] UpdateBrandRequest req)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (req.Slug is not null && req.Slug != existing.Slug && !await service.IsSlugUniqueAsync(req.Slug, id))
            return Conflict(new { message = ErrorMessages.SlugAlreadyInUse });

        if (req.Name is not null) existing.Name = req.Name;
        if (req.Slug is not null) existing.Slug = req.Slug;
        if (req.Description is not null) existing.Description = req.Description;
        if (req.IsFeatured is not null) existing.IsFeatured = req.IsFeatured;
        if (req.IsActive is not null) existing.IsActive = req.IsActive;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await service.UpdateAsync(id, existing);
        return Ok(mapper.Map<BrandResponse>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}