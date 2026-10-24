using Inventory.Domain.Actions.Entity;
using Xunit;

namespace Inventory.Tests.Domain;

/// <summary>
/// ActionEntity.Create ya no deriva el código del nombre ni valida nada
/// (commit d12fd7a: "acciones por modulo (N:M)") — Actions es ahora un
/// catálogo global reutilizable y el Code lo decide quien llama. La validación
/// de formato vive hoy en CreateActionCommandValidator, no en la entidad.
/// </summary>
public class ActionEntityTests
{
    [Theory]
    [InlineData("UsersView", "View")]
    [InlineData("UsersExportReport", "export report")]
    [InlineData("OrderItemsBulkEdit", "bulk-edit")]
    public void Create_SetsCodeAndNameVerbatim_AndStartsActive(string code, string name)
    {
        var action = ActionEntity.Create(code, name);

        Assert.Equal(code, action.Code);
        Assert.Equal(name, action.Name);
        Assert.True(action.IsActive);
    }
}
