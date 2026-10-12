using Inventory.Domain.Common.Authorization;
using Xunit;

namespace Inventory.Tests.Domain;

/// <summary>
/// Esta regla antes vivía dentro de una consulta LINQ en Infrastructure y
/// solo se podía probar con una base de datos real. Ahora es una función
/// pura de Domain.
/// </summary>
public class ModulePermissionTests
{
    [Fact]
    public void FromActionCodes_DeducesPermissionsFromTheVerbSuffix()
    {
        var permission = ModulePermission.FromActionCodes("Users", new[] { "UsersView", "UsersEdit" });

        Assert.Equal("Users", permission.Module);
        Assert.True(permission.CanView);
        Assert.False(permission.CanCreate);
        Assert.True(permission.CanEdit);
        Assert.False(permission.CanDelete);
    }

    [Fact]
    public void FromActionCodes_WithNoCodes_GrantsNothing()
    {
        var permission = ModulePermission.FromActionCodes("Users", Array.Empty<string>());

        Assert.False(permission.CanView || permission.CanCreate || permission.CanEdit || permission.CanDelete);
    }
}
