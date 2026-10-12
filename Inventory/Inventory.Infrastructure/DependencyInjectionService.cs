using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Caching;
using Inventory.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjectionService
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // IMemoryCache respalda IActiveUserCache y IPermissionCache. Los dos
        // son Scoped porque combinan el IMemoryCache (singleton) con el
        // ITenantContext (por request).
        services.AddMemoryCache();
        services.AddScoped<IActiveUserCache, ActiveUserCache>();
        services.AddScoped<IPermissionCache, PermissionCache>();

        return services;
    }
}
