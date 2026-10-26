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

    /// <summary>Omitido (null): no toca las acciones del módulo. Lista (incluso vacía): las reemplaza.</summary>
    public IReadOnlyList<int>? ActionIds { get; set; }

    /// <summary>Acciones nuevas (sin id todavía) a crear en el catálogo y
    /// agregar al conjunto final junto con <see cref="ActionIds"/>.</summary>
    public IReadOnlyList<NewModuleActionItem>? NewActions { get; set; }
}
