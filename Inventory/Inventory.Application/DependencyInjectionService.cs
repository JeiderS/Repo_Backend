

using AutoMapper;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Application.Common.Behaviors;
using Inventory.Application.Drivers.AutoMappers;
using Inventory.Application.Routes.AutoMappers;
using Inventory.Application.Vehicles.AutoMappers;
using Inventory.Application.ScheduleView.AutoMappers;
using Inventory.Application.Schedules.AutoMappers;

namespace Inventory.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        #region Mappers
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new DriversMappers());
            config.AddProfile(new RoutesMappers());
            config.AddProfile(new VehiclesMappers());
            config.AddProfile(new ScheduleViewMappers());
            config.AddProfile(new SchedulesMappers());
        });
        #endregion


        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );

        return services;
    }
}
