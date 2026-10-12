using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Modules.Entity;
using Xunit;

namespace Inventory.Tests.Domain;

public class ActionEntityTests
{
    [Theory]
    [InlineData("Users", "View", "UsersView")]
    [InlineData("Users", "export report", "UsersExportReport")]
    [InlineData("Order Items", "bulk-edit", "OrderItemsBulkEdit")]
    public void Create_BuildsCode_AsModulePlusNameInPascalCase(string moduleName, string actionName, string expectedCode)
    {
        var module = ModuleEntity.Create(moduleName, icon: null, route: null, parentId: null, sortOrder: 1);

        var result = ActionEntity.Create(module, actionName);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCode, result.Value!.Code);
        Assert.True(result.Value.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("--- ...")]
    public void Create_NameWithoutLettersOrDigits_IsRejected(string actionName)
    {
        var module = ModuleEntity.Create("Users", icon: null, route: null, parentId: null, sortOrder: 1);

        var result = ActionEntity.Create(module, actionName);

        Assert.False(result.IsSuccess);
        Assert.Equal("ACTION_INVALID_NAME", result.Error!.Code);
    }
}
