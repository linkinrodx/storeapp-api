using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using StoreApp.Api.Data;
using StoreApp.Api.DTOs.Requests;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;

namespace StoreApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<BrandResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlyActive = true)
    {
        var query = db.Brands.AsQueryable();
        if (onlyActive) query = query.Where(x => x.IsActive == true);
        var total = await query.CountAsync();
        var data = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResponse<BrandResponse>(mapper.Map<IEnumerable<BrandResponse>>(data), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BrandResponse>> GetById(Guid id)
    {
        var entity = await db.Brands.FindAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<BrandResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<BrandResponse>> Create(CreateBrandRequest req)
    {
        if (await db.Brands.AnyAsync(x => x.Slug == req.Slug))
            return Conflict(new { message = "El slug ya está en uso." });

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
        db.Brands.Add(entity);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, mapper.Map<BrandResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BrandResponse>> Update(Guid id, UpdateBrandRequest req)
    {
        var entity = await db.Brands.FindAsync(id);
        if (entity is null) return NotFound();
        if (req.Slug is not null && req.Slug != entity.Slug && await db.Brands.AnyAsync(x => x.Slug == req.Slug))
            return Conflict(new { message = "El slug ya está en uso." });

        if (req.Name is not null) entity.Name = req.Name;
        if (req.Slug is not null) entity.Slug = req.Slug;
        if (req.Description is not null) entity.Description = req.Description;
        if (req.IsFeatured is not null) entity.IsFeatured = req.IsFeatured;
        if (req.IsActive is not null) entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<BrandResponse>(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await db.Brands.FindAsync(id);
        if (entity is null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FamiliesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<FamilyResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool onlyActive = true)
    {
        var query = db.Families.AsQueryable();
        if (onlyActive) query = query.Where(x => x.IsActive == true);
        var total = await query.CountAsync();
        var data = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Label).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResponse<FamilyResponse>(mapper.Map<IEnumerable<FamilyResponse>>(data), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FamilyResponse>> GetById(Guid id)
    {
        var entity = await db.Families.FindAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<FamilyResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<FamilyResponse>> Create(CreateFamilyRequest req)
    {
        if (await db.Families.AnyAsync(x => x.Slug == req.Slug))
            return Conflict(new { message = "El slug ya está en uso." });

        var entity = new Family { Id = Guid.NewGuid(), Label = req.Label, Slug = req.Slug, SortOrder = req.SortOrder ?? 0, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        db.Families.Add(entity);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, mapper.Map<FamilyResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FamilyResponse>> Update(Guid id, UpdateFamilyRequest req)
    {
        var entity = await db.Families.FindAsync(id);
        if (entity is null) return NotFound();
        if (req.Slug is not null && req.Slug != entity.Slug && await db.Families.AnyAsync(x => x.Slug == req.Slug))
            return Conflict(new { message = "El slug ya está en uso." });

        if (req.Label is not null) entity.Label = req.Label;
        if (req.Slug is not null) entity.Slug = req.Slug;
        if (req.SortOrder is not null) entity.SortOrder = req.SortOrder;
        if (req.IsActive is not null) entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<FamilyResponse>(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await db.Families.FindAsync(id);
        if (entity is null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool onlyActive = true)
    {
        var query = db.Categories.AsQueryable();
        if (onlyActive) query = query.Where(x => x.IsActive == true);
        var total = await query.CountAsync();
        var data = await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Label).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResponse<CategoryResponse>(mapper.Map<IEnumerable<CategoryResponse>>(data), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id)
    {
        var entity = await db.Categories.FindAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<CategoryResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest req)
    {
        if (await db.Categories.AnyAsync(x => x.Slug == req.Slug))
            return Conflict(new { message = "El slug ya está en uso." });

        var entity = new Category { Id = Guid.NewGuid(), Label = req.Label, Slug = req.Slug, Icon = req.Icon, SortOrder = req.SortOrder ?? 0, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
        db.Categories.Add(entity);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, mapper.Map<CategoryResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> Update(Guid id, UpdateCategoryRequest req)
    {
        var entity = await db.Categories.FindAsync(id);
        if (entity is null) return NotFound();
        if (req.Slug is not null && req.Slug != entity.Slug && await db.Categories.AnyAsync(x => x.Slug == req.Slug))
            return Conflict(new { message = "El slug ya está en uso." });

        if (req.Label is not null) entity.Label = req.Label;
        if (req.Slug is not null) entity.Slug = req.Slug;
        if (req.Icon is not null) entity.Icon = req.Icon;
        if (req.SortOrder is not null) entity.SortOrder = req.SortOrder;
        if (req.IsActive is not null) entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<CategoryResponse>(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await db.Categories.FindAsync(id);
        if (entity is null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool onlyActive = true,
        [FromQuery] Guid? brandId = null, [FromQuery] Guid? familyId = null, [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        var query = db.Products
            .Include(x => x.Brand).Include(x => x.Family).Include(x => x.Category)
            .AsQueryable();

        if (onlyActive) query = query.Where(x => x.IsActive == true);
        if (brandId.HasValue) query = query.Where(x => x.BrandId == brandId);
        if (familyId.HasValue) query = query.Where(x => x.FamilyId == familyId);
        if (categoryId.HasValue) query = query.Where(x => x.CategoryId == categoryId);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));

        var total = await query.CountAsync();
        var data = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResponse<ProductResponse>(mapper.Map<IEnumerable<ProductResponse>>(data), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var entity = await db.Products.Include(x => x.Brand).Include(x => x.Family).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : Ok(mapper.Map<ProductResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest req)
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
        db.Products.Add(entity);
        await db.SaveChangesAsync();
        await db.Entry(entity).Reference(x => x.Brand).LoadAsync();
        await db.Entry(entity).Reference(x => x.Family).LoadAsync();
        await db.Entry(entity).Reference(x => x.Category).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, mapper.Map<ProductResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest req)
    {
        var entity = await db.Products.Include(x => x.Brand).Include(x => x.Family).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (req.Name is not null) entity.Name = req.Name;
        if (req.Price is not null) entity.Price = req.Price.Value;
        if (req.BrandId is not null) entity.BrandId = req.BrandId;
        if (req.FamilyId is not null) entity.FamilyId = req.FamilyId;
        if (req.CategoryId is not null) entity.CategoryId = req.CategoryId;
        if (req.Concentration is not null) entity.Concentration = req.Concentration;
        if (req.Description is not null) entity.Description = req.Description;
        if (req.StockQuantity is not null) entity.StockQuantity = req.StockQuantity;
        if (req.IsActive is not null) entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<ProductResponse>(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await db.Products.FindAsync(id);
        if (entity is null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ImagesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImageResponse>>> GetByEntity([FromQuery] Guid entityId, [FromQuery] string? entityType = null)
    {
        var query = db.Images.Where(x => x.EntityId == entityId && x.IsActive == true);
        if (entityType is not null) query = query.Where(x => x.EntityType == entityType);
        var data = await query.OrderBy(x => x.SortOrder).ToListAsync();
        return Ok(mapper.Map<IEnumerable<ImageResponse>>(data));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImageResponse>> GetById(Guid id)
    {
        var entity = await db.Images.FindAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<ImageResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ImageResponse>> Create(CreateImageRequest req)
    {
        var validTypes = new[] { "product", "family", "category", "brand" };
        if (!validTypes.Contains(req.EntityType))
            return BadRequest(new { message = $"entity_type debe ser uno de: {string.Join(", ", validTypes)}" });

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
        db.Images.Add(entity);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, mapper.Map<ImageResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ImageResponse>> Update(Guid id, UpdateImageRequest req)
    {
        var entity = await db.Images.FindAsync(id);
        if (entity is null) return NotFound();
        if (req.Url is not null) entity.Url = req.Url;
        if (req.AltText is not null) entity.AltText = req.AltText;
        if (req.SortOrder is not null) entity.SortOrder = req.SortOrder;
        if (req.IsPrimary is not null) entity.IsPrimary = req.IsPrimary;
        if (req.IsActive is not null) entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<ImageResponse>(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await db.Images.FindAsync(id);
        if (entity is null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProfilesController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfileResponse>> GetById(Guid id)
    {
        var entity = await db.Profiles.FindAsync(id);
        return entity is null ? NotFound() : Ok(mapper.Map<ProfileResponse>(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProfileResponse>> Update(Guid id, UpdateProfileRequest req)
    {
        var entity = await db.Profiles.FindAsync(id);
        if (entity is null) return NotFound();
        if (req.FullName is not null) entity.FullName = req.FullName;
        if (req.Phone is not null) entity.Phone = req.Phone;
        if (req.ShippingAddress is not null) entity.ShippingAddress = req.ShippingAddress;
        if (req.ShippingCity is not null) entity.ShippingCity = req.ShippingCity;
        if (req.ShippingReference is not null) entity.ShippingReference = req.ShippingReference;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<ProfileResponse>(entity));
    }
}

[ApiController]
[Route("api/users/{userId:guid}/cart")]
[Produces("application/json")]
public class CartController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemResponse>>> GetCart(Guid userId)
    {
        var items = await db.CartItems
            .Include(x => x.Product)
            .Where(x => x.UserId == userId && x.IsActive == true)
            .ToListAsync();
        return Ok(mapper.Map<IEnumerable<CartItemResponse>>(items));
    }

    [HttpPost]
    public async Task<ActionResult<CartItemResponse>> UpsertItem(Guid userId, UpsertCartItemRequest req)
    {
        var existing = await db.CartItems.FindAsync(userId, req.ProductId);
        if (existing is not null)
        {
            existing.Quantity = req.Quantity;
            existing.IsActive = true;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            var item = new CartItem
            {
                UserId = userId,
                ProductId = req.ProductId,
                Quantity = req.Quantity,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            db.CartItems.Add(item);
        }
        await db.SaveChangesAsync();
        var result = await db.CartItems.Include(x => x.Product).FirstAsync(x => x.UserId == userId && x.ProductId == req.ProductId);
        return Ok(mapper.Map<CartItemResponse>(result));
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid userId, Guid productId)
    {
        var item = await db.CartItems.FindAsync(userId, productId);
        if (item is null) return NotFound();
        db.CartItems.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        var items = db.CartItems.Where(x => x.UserId == userId);
        db.CartItems.RemoveRange(items);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? userId = null, [FromQuery] string? status = null)
    {
        var query = db.Orders.Include(x => x.Items).AsQueryable();
        if (userId.HasValue) query = query.Where(x => x.UserId == userId);
        if (status is not null) query = query.Where(x => x.Status == status);
        var total = await query.CountAsync();
        var data = await query.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResponse<OrderResponse>(mapper.Map<IEnumerable<OrderResponse>>(data), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var entity = await db.Orders.Include(x => x.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? NotFound() : Ok(mapper.Map<OrderResponse>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromQuery] Guid userId, CreateOrderRequest req)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
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
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
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
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, mapper.Map<OrderResponse>(order));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(Guid id, UpdateOrderStatusRequest req)
    {
        var validStatuses = new[] { "pending", "processing", "shipped", "delivered", "cancelled" };
        if (!validStatuses.Contains(req.Status))
            return BadRequest(new { message = $"Estado inválido. Debe ser uno de: {string.Join(", ", validStatuses)}" });

        var entity = await db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        entity.Status = req.Status;
        if (req.Status == "shipped") entity.ShippedAt = DateTimeOffset.UtcNow;
        if (req.Status == "delivered") entity.DeliveredAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return Ok(mapper.Map<OrderResponse>(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var entity = await db.Orders.FindAsync(id);
        if (entity is null) return NotFound();
        if (entity.Status is "shipped" or "delivered")
            return BadRequest(new { message = "No se puede cancelar un pedido que ya fue enviado o entregado." });
        entity.Status = "cancelled";
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/users/{userId:guid}/wishlist")]
[Produces("application/json")]
public class WishlistController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistResponse>>> GetWishlist(Guid userId)
    {
        var items = await db.Wishlists
            .Include(x => x.Product)
            .Where(x => x.UserId == userId && x.IsActive == true)
            .ToListAsync();
        return Ok(mapper.Map<IEnumerable<WishlistResponse>>(items));
    }

    [HttpPost("{productId:guid}")]
    public async Task<ActionResult<WishlistResponse>> AddToWishlist(Guid userId, Guid productId)
    {
        var existing = await db.Wishlists.FindAsync(userId, productId);
        if (existing is not null)
        {
            existing.IsActive = true;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            db.Wishlists.Add(new Wishlist { UserId = userId, ProductId = productId, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow });
        }
        await db.SaveChangesAsync();
        var result = await db.Wishlists.Include(x => x.Product).FirstAsync(x => x.UserId == userId && x.ProductId == productId);
        return Ok(mapper.Map<WishlistResponse>(result));
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid userId, Guid productId)
    {
        var item = await db.Wishlists.FindAsync(userId, productId);
        if (item is null) return NotFound();
        db.Wishlists.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        return Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow });
    }
}
