using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Caching;
using Inventory.Infrastructure.Media;
using Inventory.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjectionService
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddHttpClient<ICloudinaryImageUploader, CloudinaryImageUploader>();

        // IMemoryCache respalda IActiveUserCache y IPermissionCache. Los dos
        // son Scoped porque combinan el IMemoryCache (singleton) con el
        // ITenantContext (por request).
        services.AddMemoryCache();
        services.AddScoped<IActiveUserCache, ActiveUserCache>();
        services.AddScoped<IPermissionCache, PermissionCache>();

        return services;
    }
}
