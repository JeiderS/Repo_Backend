using Inventory.Domain.Modules.Entity;

namespace Inventory.Application.Modules.Dto;

public class ModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }

    public static ModuleDto FromEntity(ModuleEntity module) => new()
    {
        Id = module.Id,
        Name = module.Name,
        Icon = module.Icon,
        Route = module.Route,
        ParentId = module.ParentId,
        IsActive = module.IsActive,
        SortOrder = module.SortOrder
    };
}
