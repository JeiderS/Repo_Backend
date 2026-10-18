using Inventory.Domain.Roles.Entity;

namespace Inventory.Application.Roles.Dto;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public IReadOnlyList<RoleModuleActionGrant> Grants { get; set; } = Array.Empty<RoleModuleActionGrant>();

    public static RoleDto FromEntity(RoleEntity role, IReadOnlyList<RoleModuleActionGrant> grants) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Grants = grants
    };
}
