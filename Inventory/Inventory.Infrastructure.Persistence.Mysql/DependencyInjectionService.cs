using Inventory.Domain.Drivers.DomainDrivers;
using Inventory.Domain.Routes.DomainRoutes;
using Inventory.Domain.Schedules.DomainSchedules;
using Inventory.Domain.ScheduleView.DomainScheduleView;
using Inventory.Domain.Vehicles.DomainVehicles;
using Inventory.Domain.Users.DomainUsers;
using Inventory.Infrastructure.Persistence.Mysql.Drivers.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.Users.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.Routes.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.Schedules.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.ScheduleView.DomainService.Impl;
using Inventory.Infrastructure.Persistence.Mysql.Vehicles.DomainService.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.Persistence.Mysql
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

            services.AddScoped<ISchedulesCreateService, SchedulesCreateService>();
            services.AddScoped<ISchedulesDeleteService, SchedulesDeleteService>();
            services.AddScoped<ISchedulesGetAllService, SchedulesGetAllService>();
            services.AddScoped<ISchedulesGetByIdService, SchedulesGetByIdService>();
            services.AddScoped<ISchedulesUpdateService, SchedulesUpdateService>();

            services.AddScoped<IScheduleViewGetAllService, ScheduleViewGetAllService>();

            services.AddScoped<IUserGetByEmailService, UserGetByEmailService>();
            services.AddScoped<IUserCreateService, UserCreateService>();

            return services;
        }

    }
}
