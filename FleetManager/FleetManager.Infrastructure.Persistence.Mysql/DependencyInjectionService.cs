using FleetManager.Domain.Drivers.DomainDrivers;
using FleetManager.Domain.Routes.DomainRoutes;
using FleetManager.Domain.Vehicles.DomainVehicles;
using FleetManager.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;
using FleetManager.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;
using FleetManager.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FleetManager.Infrastructure.Persistence.Mysql
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IDriversGetAllService, DriversGetAllServices>();
            services.AddScoped<IDriversCreateService, DriversCreateService>();
            services.AddScoped<IDriversGetByIdService, DriversGetByIdService>();
            services.AddScoped<IDriversUpdateService, DriversUpdateService>();
            services.AddScoped<IDriversDeleteService, DriversDeleteService>();

            services.AddScoped<IRoutesGetAllService, RoutesGetAllServices>();
            services.AddScoped<IRoutesCreateService, RoutesCreateService>();
            services.AddScoped<IRoutesGetByIdService, RoutesGetByIdService>();
            services.AddScoped<IRoutesUpdateService, RoutesUpdateService>();
            services.AddScoped<IRoutesDeleteService, RoutesDeleteService>();

            services.AddScoped<IVehiclesGetAllService, VehiclesGetAllServices>();
            services.AddScoped<IVehiclesCreateService, VehiclesCreateService>();
            services.AddScoped<IVehiclesGetByIdService, VehiclesGetByIdService>();
            services.AddScoped<IVehiclesUpdateService, VehiclesUpdateService>();
            services.AddScoped<IVehiclesDeleteService, VehiclesDeleteService>();
            return services;
        }

    }
}
