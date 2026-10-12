using Inventory.Domain.Roles.Entity;
using Xunit;

namespace Inventory.Tests.Domain;

public class RoleEntityTests
{
    [Fact]
    public void Create_RoleFromTheApplication_IsNeverSystemAdmin()
    {
        var role = RoleEntity.Create("Bodega");

        Assert.False(role.IsSystemAdmin);
    }
}
