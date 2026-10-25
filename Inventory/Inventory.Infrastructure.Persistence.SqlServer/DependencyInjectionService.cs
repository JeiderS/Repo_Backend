using Inventory.Domain.Actions.Repositories;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Modules.Repositories;
using Inventory.Domain.RefreshTokens.Repositories;
using Inventory.Domain.Roles.Repositories;
using Inventory.Domain.Users.Repositories;
using Inventory.Infrastructure.Persistence.SqlServer.Common.Persistence;
using Inventory.Infrastructure.Persistence.SqlServer.Context;
using Inventory.Infrastructure.Persistence.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.Persistence.SqlServer
{
    public static class DependencyInjectionService
    {
        /// <summary>
        /// Registra todo lo de persistencia: el DbContext, la unidad de trabajo
        /// y los repositorios.
        /// </summary>
        /// <param name="connectionStringFactory">
        /// Cómo obtener la cadena de conexión en cada request. Se recibe desde
        /// afuera (Program.cs) porque depende del tenant resuelto, y esta capa
        /// no debe conocer cómo se resuelve un tenant.
        /// </param>
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            Func<IServiceProvider, string> connectionStringFactory)
        {
            // DbContext por request: la cadena de conexión cambia según el
            // tenant, así que se calcula cada vez a partir del IServiceProvider.
            services.AddDbContext<DataBaseContext>((sp, options) =>
                options.UseSqlServer(connectionStringFactory(sp)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IActionRepository, ActionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }
    }
}
