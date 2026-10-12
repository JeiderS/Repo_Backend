using FluentValidation;
using MediatR;

namespace Inventory.Application.Common.Behaviors;

/// <summary>
/// Se ejecuta antes de cada handler (pipeline de MediatR). Si el request
/// tiene validadores y alguno falla, corta la ejecución con una
/// <see cref="ValidationException"/>; el GlobalExceptionHandler de la Api la
/// convierte en un 400 con la lista de errores.
///
/// ¿Por qué excepción y no Result? Porque este behavior es genérico: no sabe
/// si el handler devuelve Result&lt;T, Error&gt;, una lista o un DTO. Una
/// excepción sirve para todos. Los errores ESPERADOS del negocio (email
/// repetido, rol inexistente...) siguen usando Result.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}
