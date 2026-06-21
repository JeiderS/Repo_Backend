using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Application.Common.Behaviors;

namespace Inventory.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );

        return services;
    }
}
