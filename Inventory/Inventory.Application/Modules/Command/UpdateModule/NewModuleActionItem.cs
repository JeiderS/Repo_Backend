namespace Inventory.Application.Modules.Command.UpdateModule;

/// <summary>Una accion que aun no existe en el catalogo y debe crearse antes
/// de asignarla al modulo, dentro del mismo request de UpdateModule.</summary>
public sealed record NewModuleActionItem(string Code, string Name);
