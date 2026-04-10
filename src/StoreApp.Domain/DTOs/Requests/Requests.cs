using System.ComponentModel.DataAnnotations;

namespace StoreApp.Domain.DTOs.Requests;

// Brand
public record CreateBrandRequest(
    [Required] string Name,
    [Required] string Slug,
    string? Description,
    bool? IsFeatured);

public record UpdateBrandRequest(
    string? Name,
    string? Slug,
    string? Description,
    bool? IsFeatured,
    bool? IsActive);

// Family
public record CreateFamilyRequest([Required] string Label, [Required] string Slug, int? SortOrder);
public record UpdateFamilyRequest(string? Label, string? Slug, int? SortOrder, bool? IsActive);

// Category
public record CreateCategoryRequest([Required] string Label, [Required] string Slug, string? Icon, int? SortOrder);
public record UpdateCategoryRequest(string? Label, string? Slug, string? Icon, int? SortOrder, bool? IsActive);

// Product
public record CreateProductRequest(
    [Required] string Name,
    [Required][Range(0.01, double.MaxValue)] decimal Price,
    Guid? BrandId,
    Guid? FamilyId,
    Guid? CategoryId,
    string? Concentration,
    string? Description,
    int? StockQuantity);

public record UpdateProductRequest(
    string? Name,
    decimal? Price,
    Guid? BrandId,
    Guid? FamilyId,
    Guid? CategoryId,
    string? Concentration,
    string? Description,
    int? StockQuantity,
    bool? IsActive);

// Image
public record CreateImageRequest(
    [Required] Guid EntityId,
    [Required] string EntityType,
    [Required] string Url,
    string? AltText,
    int? SortOrder,
    bool? IsPrimary);

public record UpdateImageRequest(string? Url, string? AltText, int? SortOrder, bool? IsPrimary, bool? IsActive);

// UserProfile
public record UpdateProfileRequest(
    string? FullName,
    string? Phone,
    string? ShippingAddress,
    string? ShippingCity,
    string? ShippingReference);

// CartItem
public record UpsertCartItemRequest([Required] Guid ProductId, [Required][Range(1, 999)] int Quantity);

// Order
public record CreateOrderRequest(
    [Required] decimal Subtotal,
    [Required] decimal Total,
    decimal? ShippingCost,
    decimal? Tax,
    string? ShippingAddress,
    string? ShippingCity,
    string? ShippingReference,
    string? ShippingMethod,
    string? PaymentMethod,
    string? PaymentReference,
    [Required] IEnumerable<CreateOrderItemRequest> Items);

public record CreateOrderItemRequest(
    [Required] Guid ProductId,
    [Required] string ProductName,
    string? BrandName,
    [Required] decimal UnitPrice,
    [Required] int Quantity,
    [Required] decimal Subtotal);

public record UpdateOrderStatusRequest([Required] string Status);
