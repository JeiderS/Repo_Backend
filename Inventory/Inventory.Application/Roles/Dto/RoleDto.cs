using Inventory.Domain.Roles.Entity;

namespace Inventory.Application.Roles.Dto;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public IReadOnlyList<int> ActionIds { get; set; } = Array.Empty<int>();

    public static RoleDto FromEntity(RoleEntity role, IReadOnlyList<int> actionIds) => new()
    {
        Id = role.Id,
        Name = role.Name,
        ActionIds = actionIds
    };
}
