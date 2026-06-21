using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Inventory.Application.Common.Behaviors;

public class ValidationBehaviour<TRequest,
    TResponse>(IEnumerable<IValidator<TRequest>> validators)

    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);
            List<ValidationFailure> list = (await Task.WhenAll(validators.Select((v) => v.ValidateAsync(context, cancellationToken)))).Where((r) => r.Errors.Any()).SelectMany((r) => r.Errors).ToList();
            if (list.Any())
            {
                throw new ValidationException(list);
            }
        }

        return await next();
    }
}
