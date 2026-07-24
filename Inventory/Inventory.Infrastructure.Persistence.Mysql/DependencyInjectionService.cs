using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Modules.DomainModules;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Domain.UserProfile.DomainUserProfile;
using Inventory.Infrastructure.Persistence.Mysql.Common.Persistence;
using Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.UserProfile.DomainService.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.Persistence.Mysql
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserGetByEmailService, UserGetByEmailService>();
            services.AddScoped<IUserCreateService, UserCreateService>();
            services.AddScoped<IUserProfileCreateService, UserProfileCreateService>();
            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            services.AddScoped<IModuleMenuService, ModuleMenuService>();
            services.AddScoped<IModuleListService, ModuleListService>();
            services.AddScoped<IModuleCreateService, ModuleCreateService>();
            services.AddScoped<IModuleGetByIdService, ModuleGetByIdService>();
            services.AddScoped<IModuleUpdateService, ModuleUpdateService>();

            return services;
        }

    }
}
