using Inventory.Domain.Modules.Entity;
using Xunit;

namespace Inventory.Tests.Domain;

public class ModuleEntityTests
{
    private static ModuleEntity NewModule() =>
        ModuleEntity.Create("Inventario", icon: null, route: "/inventario", parentId: null, sortOrder: 1);

    [Fact]
    public void Create_NewModule_StartsActive()
    {
        Assert.True(NewModule().IsActive);
    }

    [Fact]
    public void Update_ModuleAsItsOwnParent_IsRejected_AndNothingChanges()
    {
        var module = NewModule();

        var result = module.Update("Otro nombre", icon: null, route: null, parentId: module.Id, sortOrder: 5);

        Assert.False(result.IsSuccess);
        Assert.Equal("MODULE_INVALID_PARENT", result.Error!.Code);
        Assert.Equal("Inventario", module.Name); // la entidad no quedó a medio cambiar
    }

    [Fact]
    public void Deactivate_ActiveModule_ChangesState_SecondTimeDoesNothing()
    {
        var module = NewModule();

        Assert.True(module.Deactivate());   // activo -> inactivo
        Assert.False(module.IsActive);
        Assert.False(module.Deactivate());  // ya estaba inactivo: no hace nada
    }
}
