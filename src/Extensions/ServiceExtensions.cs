using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Implementations;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Extensions;

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