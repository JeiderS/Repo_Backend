using Inventory.Application.Actions.Dto;
using Inventory.Domain.Actions.Repositories;
using MediatR;

namespace Inventory.Application.Actions.Query.GetActions;

public class GetActionsQueryHandler(IActionRepository actionRepository)
    : IRequestHandler<GetActionsQuery, IReadOnlyList<ActionDto>>
{
    public async Task<IReadOnlyList<ActionDto>> Handle(GetActionsQuery request, CancellationToken cancellationToken)
    {
        var actions = await actionRepository.GetAllAsync();
        return actions.Select(ActionDto.FromEntity).ToList();
    }
}
