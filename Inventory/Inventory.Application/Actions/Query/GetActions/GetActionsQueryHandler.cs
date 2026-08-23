using Inventory.Application.Actions.Dto;
using Inventory.Domain.Actions.DomainActions;
using MediatR;

namespace Inventory.Application.Actions.Query.GetActions;

public class GetActionsQueryHandler(IActionListService actionListService)
    : IRequestHandler<GetActionsQuery, IReadOnlyList<ActionDto>>
{
    public async Task<IReadOnlyList<ActionDto>> Handle(GetActionsQuery request, CancellationToken cancellationToken)
    {
        var actions = await actionListService.GetAllAsync();
        return actions.Select(ActionDto.FromEntity).ToList();
    }
}
