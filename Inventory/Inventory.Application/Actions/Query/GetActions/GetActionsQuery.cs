using Inventory.Application.Actions.Dto;
using MediatR;

namespace Inventory.Application.Actions.Query.GetActions;

public record GetActionsQuery : IRequest<IReadOnlyList<ActionDto>>;
