using Inventory.Domain.Actions.Entity;

namespace Inventory.Application.Actions.Dto;

public class ActionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int ModuleId { get; set; }
    public string? ModuleName { get; set; }

    public static ActionDto FromEntity(ActionEntity action) => new()
    {
        Id = action.Id,
        Code = action.Code,
        Name = action.Name,
        ModuleId = action.ModuleId,
        ModuleName = action.Module?.Name
    };
}
