using AutoMapper;
using StoreApp.Api.DTOs.Responses;
using StoreApp.Api.Models;

namespace StoreApp.Api.Mappings;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<Brand, BrandResponse>()
            .ConstructUsing(src => new BrandResponse(
                src.Id, src.Name, src.Slug, src.Description,
                src.IsFeatured, src.IsActive, src.CreatedAt));

        CreateMap<Family, FamilyResponse>()
            .ConstructUsing(src => new FamilyResponse(
                src.Id, src.Label, src.Slug, src.SortOrder, src.IsActive, src.CreatedAt));

        CreateMap<Category, CategoryResponse>()
            .ConstructUsing(src => new CategoryResponse(
                src.Id, src.Label, src.Slug, src.Icon, src.SortOrder, src.IsActive, src.CreatedAt));

        CreateMap<Models.Product, ProductResponse>()
            .ConstructUsing(src => new ProductResponse(
                src.Id, src.Name, src.Price, src.Concentration, src.Description,
                src.StockQuantity, src.IsActive,
                src.BrandId, src.Brand != null ? src.Brand.Name : null,
                src.FamilyId, src.Family != null ? src.Family.Label : null,
                src.CategoryId, src.Category != null ? src.Category.Label : null,
                src.CreatedAt));

        CreateMap<Image, ImageResponse>()
            .ConstructUsing(src => new ImageResponse(
                src.Id, src.EntityId, src.EntityType, src.Url,
                src.AltText, src.SortOrder, src.IsPrimary, src.IsActive));

        CreateMap<Models.UserProfile, ProfileResponse>()
            .ConstructUsing(src => new ProfileResponse(
                src.Id, src.Email, src.FullName, src.Phone,
                src.ShippingAddress, src.ShippingCity, src.ShippingReference, src.IsActive));

        CreateMap<CartItem, CartItemResponse>()
            .ConstructUsing(src => new CartItemResponse(
                src.UserId, src.ProductId, src.Quantity,
                src.Product != null ? src.Product.Name : null,
                src.Product != null ? src.Product.Price : (decimal?)null,
                src.IsActive));

        CreateMap<OrderItem, OrderItemResponse>()
            .ConstructUsing(src => new OrderItemResponse(
                src.OrderId, src.ProductId, src.ProductName,
                src.BrandName, src.UnitPrice, src.Quantity, src.Subtotal));

        CreateMap<Order, OrderResponse>()
            .ConstructUsing((src, ctx) => new OrderResponse(
                src.Id, src.UserId, src.Status,
                src.Subtotal, src.ShippingCost, src.Tax, src.Total,
                src.ShippingAddress, src.ShippingCity, src.ShippingMethod,
                src.PaymentMethod, src.PaymentStatus, src.PaymentReference,
                src.IsActive, src.CreatedAt, src.ShippedAt, src.DeliveredAt,
                ctx.Mapper.Map<IEnumerable<OrderItemResponse>>(src.Items)));

        CreateMap<Wishlist, WishlistResponse>()
            .ConstructUsing(src => new WishlistResponse(
                src.UserId, src.ProductId,
                src.Product != null ? src.Product.Name : null,
                src.Product != null ? src.Product.Price : (decimal?)null,
                src.IsActive, src.CreatedAt));
    }
}
