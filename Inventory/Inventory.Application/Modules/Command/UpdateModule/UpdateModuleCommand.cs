using Inventory.Application.Modules.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Modules.Command.UpdateModule;

public class UpdateModuleCommand : IRequest<Result<ModuleDto, Error>>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? ParentId { get; set; }
    public int SortOrder { get; set; }
}
