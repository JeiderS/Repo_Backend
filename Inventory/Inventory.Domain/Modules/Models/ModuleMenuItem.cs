namespace Inventory.Domain.Modules.Models;

public record ModuleMenuItem(
    string Nombre,
    string? Icon,
    string? Route,
    IReadOnlyList<ModuleMenuItem> Children);
