using Inventory.Application.Roles.Dto;
using Inventory.Domain.Roles.DomainRoles;
using MediatR;

namespace Inventory.Application.Roles.Query.GetRoles;

public class GetRolesQueryHandler(
    IRoleListService roleListService,
    IRoleActionAssignService roleActionAssignService) : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleDto>>
{
    public async Task<IReadOnlyList<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleListService.GetAllAsync();

        var result = new List<RoleDto>(roles.Count);
        foreach (var role in roles)
        {
            var actionIds = await roleActionAssignService.GetActionIdsAsync(role.Id);
            result.Add(RoleDto.FromEntity(role, actionIds));
        }

        return result;
    }
}
