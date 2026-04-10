using StoreApp.Domain.Repositories.Interfaces;
using StoreApp.Domain.Services.Implementations;
using StoreApp.Domain.Services.Interfaces;

namespace StoreApp.Domain.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repositories.Implementations.GenericRepository<>));

        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IFamilyService, FamilyService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IWishlistService, WishlistService>();

        return services;
    }
}