

using AutoMapper;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FleetManager.Application.Common.Behaviors;
using FleetManager.Application.Drivers.AutoMappers;
using FleetManager.Application.Routes.AutoMappers;
using FleetManager.Application.Vehicles.AutoMappers;

namespace FleetManager.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        #region Mappers
        var mapper = new MapperConfiguration(config =>
        {
            config.AddProfile(new DriversMappers());
            config.AddProfile(new RoutesMappers());
            config.AddProfile(new VehiclesMappers());

        });
        services.AddSingleton(mapper.CreateMapper());
        #endregion


        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );

        return services;
    }
}
