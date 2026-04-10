using StoreApp.Api.Repositories.Interfaces;
using StoreApp.Api.Services.Interfaces;

namespace StoreApp.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repositories.Implementations.GenericRepository<>));
        return services;
    }
}