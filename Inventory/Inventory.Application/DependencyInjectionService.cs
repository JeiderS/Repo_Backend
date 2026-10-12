using System.Globalization;
using System.Reflection;
using FluentValidation;
using Inventory.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Registra automáticamente todos los AbstractValidator<T> de este proyecto.
        // Antes no había ninguno, así que ValidationBehavior nunca validaba nada.
        services.AddValidatorsFromAssembly(assembly);

        // Mensajes de validación en español (FluentValidation trae la traducción).
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");

        // Pipeline (patrón Decorator): antes de CADA handler se ejecuta la validación.
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
