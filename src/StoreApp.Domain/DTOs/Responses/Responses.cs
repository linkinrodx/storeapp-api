namespace StoreApp.Domain.DTOs.Responses;

public record BrandResponse(Guid Id, string Name, string Slug, string? Description, bool? IsFeatured, bool? IsActive, DateTimeOffset? CreatedAt);
public record FamilyResponse(Guid Id, string Label, string Slug, int? SortOrder, bool? IsActive, DateTimeOffset? CreatedAt);
public record CategoryResponse(Guid Id, string Label, string Slug, string? Icon, int? SortOrder, bool? IsActive, DateTimeOffset? CreatedAt);

public record ProductResponse(
    Guid Id, string Name, decimal Price, string? Concentration, string? Description,
    int? StockQuantity, bool? IsActive,
    Guid? BrandId, string? BrandName,
    Guid? FamilyId, string? FamilyLabel,
    Guid? CategoryId, string? CategoryLabel,
    DateTimeOffset? CreatedAt);

public record ImageResponse(Guid Id, Guid EntityId, string EntityType, string Url, string? AltText, int? SortOrder, bool? IsPrimary, bool? IsActive);

public record ProfileResponse(Guid Id, string Email, string? FullName, string? Phone, string? ShippingAddress, string? ShippingCity, string? ShippingReference, bool? IsActive);

public record CartItemResponse(Guid UserId, Guid ProductId, int Quantity, string? ProductName, decimal? ProductPrice, bool? IsActive);

public record OrderItemResponse(Guid OrderId, Guid ProductId, string ProductName, string? BrandName, decimal UnitPrice, int Quantity, decimal Subtotal);

public record OrderResponse(
    Guid Id, Guid? UserId, string Status,
    decimal Subtotal, decimal? ShippingCost, decimal? Tax, decimal Total,
    string? ShippingAddress, string? ShippingCity, string? ShippingMethod,
    string? PaymentMethod, string? PaymentStatus, string? PaymentReference,
    bool? IsActive, DateTimeOffset? CreatedAt, DateTimeOffset? ShippedAt, DateTimeOffset? DeliveredAt,
    IEnumerable<OrderItemResponse> Items);

public record WishlistResponse(Guid UserId, Guid ProductId, string? ProductName, decimal? ProductPrice, bool? IsActive, DateTimeOffset? CreatedAt);

public record PagedResponse<T>(IEnumerable<T> Data, int Total, int Page, int PageSize);
