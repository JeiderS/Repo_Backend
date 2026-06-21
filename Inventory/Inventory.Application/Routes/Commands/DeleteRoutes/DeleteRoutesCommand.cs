using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Common.Results;
using MediatR;

namespace Inventory.Application.Routes.Commands.DeleteRoutes;

public record DeleteRoutesCommand(int Id) : IRequest<Result<VoidResult, Error>>;
