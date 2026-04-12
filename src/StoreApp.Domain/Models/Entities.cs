namespace StoreApp.Domain.Models;

public abstract class AuditableEntity
{
    public bool? IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class UserProfile : AuditableEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? ShippingAddress { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingReference { get; set; }
}

public class Brand : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public bool? IsFeatured { get; set; } = false;
    public ICollection<Product> Products { get; set; } = [];
}

public class Family : AuditableEntity
{
    public Guid Id { get; set; }
    public string Label { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int? SortOrder { get; set; } = 0;
    public ICollection<Product> Products { get; set; } = [];
}

public class Category : AuditableEntity
{
    public Guid Id { get; set; }
    public string Label { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Icon { get; set; }
    public int? SortOrder { get; set; } = 0;
    public ICollection<Product> Products { get; set; } = [];
}

public class Product : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? BrandId { get; set; }
    public Guid? FamilyId { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? Concentration { get; set; }
    public string? Description { get; set; }
    public int? StockQuantity { get; set; } = 0;
    public Brand? Brand { get; set; }
    public Family? Family { get; set; }
    public Category? Category { get; set; }
}

public class Image : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = null!;  // product | family | category | brand
    public string Url { get; set; } = null!;
    public string? AltText { get; set; }
    public int? SortOrder { get; set; } = 0;
    public bool? IsPrimary { get; set; } = false;
}

public class CartItem : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public Product? Product { get; set; }
}

public class Order : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Status { get; set; } = "pending";  // pending | processing | shipped | delivered | cancelled
    public decimal Subtotal { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal? Tax { get; set; }
    public decimal Total { get; set; }
    public string? ShippingAddress { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingReference { get; set; }
    public string? ShippingMethod { get; set; }  // standard | express
    public string? PaymentMethod { get; set; }   // card | cash_on_delivery
    public string? PaymentStatus { get; set; } = "pending";  // pending | paid | failed
    public string? PaymentReference { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}

public class OrderItem : AuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? BrandName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}

public class Wishlist : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}
