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
public class ProductsController(IProductService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlyActive = true,
        [FromQuery] Guid? brandId = null,
        [FromQuery] Guid? familyId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        var result = await service.GetAllAsync(page, pageSize, onlyActive, brandId, familyId, categoryId, search);
        return Ok(mapper.Map<PagedResponse<ProductResponse>>(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<ProductResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest req)
    {
        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Price = req.Price,
            BrandId = req.BrandId,
            FamilyId = req.FamilyId,
            CategoryId = req.CategoryId,
            Concentration = req.Concentration,
            Description = req.Description,
            StockQuantity = req.StockQuantity ?? 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        var created = await service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, mapper.Map<ProductResponse>(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(Guid id, [FromBody] UpdateProductRequest req)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (req.Name is not null) existing.Name = req.Name;
        if (req.Price is not null) existing.Price = req.Price.Value;
        if (req.BrandId is not null) existing.BrandId = req.BrandId;
        if (req.FamilyId is not null) existing.FamilyId = req.FamilyId;
        if (req.CategoryId is not null) existing.CategoryId = req.CategoryId;
        if (req.Concentration is not null) existing.Concentration = req.Concentration;
        if (req.Description is not null) existing.Description = req.Description;
        if (req.StockQuantity is not null) existing.StockQuantity = req.StockQuantity;
        if (req.IsActive is not null) existing.IsActive = req.IsActive;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await service.UpdateAsync(id, existing);
        return Ok(mapper.Map<ProductResponse>(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}