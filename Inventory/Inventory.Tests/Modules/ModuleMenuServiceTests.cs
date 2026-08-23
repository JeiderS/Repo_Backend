using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Modules.Entity;
using Inventory.Domain.Modules.Models;
using Inventory.Domain.RoleActions.Entity;
using Inventory.Domain.Roles.Entity;
using Inventory.Domain.Users.Entity;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Inventory.Infrastructure.Persistence.Mysql.Modules.DomainService.Impl;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventory.Tests.Modules;

/// <summary>
/// Task 4.2 (Phase 4, Checkpoint B tests) — real-DB service test for
/// <see cref="ModuleMenuService"/>'s D5 rule (design.md): a module is
/// visible only when the role holds &gt;= 1 of its 4 Actions AND
/// (!Module.RequiresSystemAdmin || Role.IsSystemAdmin); the pre-existing
/// "container with a visible descendant" rule must still hold.
///
/// This project (Inventory.Tests.csproj) does not reference an EF Core
/// InMemory provider, only the real SQL Server-backed
/// <see cref="DataBaseContext"/> via Inventory.Infrastructure.Persistence.Mysql,
/// so per design.md's Testing Strategy "In-memory/real DB service test"
/// option this exercises the real context directly against
/// INVENTORY_TEST_DEFAULT_CONNECTION — same real-per-tenant-DB dependency
/// and BLOCKED-skip convention TenantLoginTests/UserManagementTests already
/// use. Every row this test creates is GUID-suffixed (unique Module.Name /
/// Action.Code / Role.Name / User.Email) and removed in a finally block, so
/// re-runs never collide and nothing durable is left in the shared local
/// dev DB.
/// </summary>
public class ModuleMenuServiceTests
{
    [Fact]
    public async Task ModuleWithOneOfFourGrantedActions_IsVisible()
    {
        SkipUnlessRealDatabasesConfigured();

        await using var context = new DataBaseContext(BuildOptions());
        var suffix = Guid.NewGuid().ToString("N");
        var module = await CreateModuleWithActionsAsync(context, $"MMSVis-{suffix}", requiresSystemAdmin: false);
        var role = await CreateRoleAsync(context, $"MMSVisRole-{suffix}");
        await GrantActionsAsync(context, role.Id, new[] { module.Actions["View"] });
        var user = await CreateUserAsync(context, $"mms-vis-{suffix}@example.com", role.Id);

        try
        {
            var menu = await new ModuleMenuService(context).GetMenuForUserAsync(user.Id);

            Assert.NotNull(FindByName(menu, module.Module.Name));
        }
        finally
        {
            await CleanupAsync(role.Id, new[] { module.Module.Id }, user.Id);
        }
    }

    [Fact]
    public async Task RequiresSystemAdminModule_HiddenFromNonSystemAdminRole_EvenHoldingAllFourActions()
    {
        SkipUnlessRealDatabasesConfigured();

        await using var context = new DataBaseContext(BuildOptions());
        var suffix = Guid.NewGuid().ToString("N");
        var module = await CreateModuleWithActionsAsync(context, $"MMSAdmin-{suffix}", requiresSystemAdmin: true);
        var role = await CreateRoleAsync(context, $"MMSAdminRole-{suffix}");
        await GrantActionsAsync(context, role.Id, module.Actions.Values);
        var user = await CreateUserAsync(context, $"mms-admin-{suffix}@example.com", role.Id);

        try
        {
            var menu = await new ModuleMenuService(context).GetMenuForUserAsync(user.Id);

            // Holding every Action the module has is not enough — only
            // Roles.IsSystemAdmin (SQL-only, D1) clears RequiresSystemAdmin.
            Assert.Null(FindByName(menu, module.Module.Name));
        }
        finally
        {
            await CleanupAsync(role.Id, new[] { module.Module.Id }, user.Id);
        }
    }

    [Fact]
    public async Task ContainerModule_StillShown_WhenOnlyItsDescendantIsVisible()
    {
        SkipUnlessRealDatabasesConfigured();

        await using var context = new DataBaseContext(BuildOptions());
        var suffix = Guid.NewGuid().ToString("N");
        var parent = await CreateModuleWithActionsAsync(context, $"MMSParent-{suffix}", requiresSystemAdmin: false);
        var child = await CreateModuleWithActionsAsync(context, $"MMSChild-{suffix}", requiresSystemAdmin: false, parentId: parent.Module.Id);
        var role = await CreateRoleAsync(context, $"MMSContainerRole-{suffix}");
        // Grant only the CHILD's View action — the parent has zero granted
        // Actions of its own and must still render as a container.
        await GrantActionsAsync(context, role.Id, new[] { child.Actions["View"] });
        var user = await CreateUserAsync(context, $"mms-container-{suffix}@example.com", role.Id);

        try
        {
            var menu = await new ModuleMenuService(context).GetMenuForUserAsync(user.Id);

            var parentItem = FindByName(menu, parent.Module.Name);
            Assert.NotNull(parentItem);
            Assert.Contains(parentItem!.Children, c => c.Nombre == child.Module.Name);
        }
        finally
        {
            await CleanupAsync(role.Id, new[] { child.Module.Id, parent.Module.Id }, user.Id);
        }
    }

    private static ModuleMenuItem? FindByName(IReadOnlyList<ModuleMenuItem> items, string name)
    {
        foreach (var item in items)
        {
            if (item.Nombre == name)
                return item;

            var found = FindByName(item.Children, name);
            if (found is not null)
                return found;
        }

        return null;
    }

    private static DbContextOptions<DataBaseContext> BuildOptions()
    {
        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        return new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
    }

    private static async Task<(ModuleEntity Module, Dictionary<string, ActionEntity> Actions)> CreateModuleWithActionsAsync(
        DataBaseContext context, string name, bool requiresSystemAdmin, int? parentId = null)
    {
        var module = new ModuleEntity
        {
            Name = name,
            Route = name.ToLowerInvariant(),
            ParentId = parentId,
            IsActive = true,
            SortOrder = 999,
            RequiresSystemAdmin = requiresSystemAdmin,
        };
        context.Modules.Add(module);
        await context.SaveChangesAsync();

        var actions = new Dictionary<string, ActionEntity>();
        foreach (var verb in new[] { "View", "Create", "Edit", "Delete" })
        {
            var action = new ActionEntity
            {
                ModuleId = module.Id,
                Code = $"{name}{verb}",
                Name = $"{name} - {verb}",
                IsActive = true,
            };
            context.Actions.Add(action);
            actions[verb] = action;
        }

        await context.SaveChangesAsync();
        return (module, actions);
    }

    private static async Task<RoleEntity> CreateRoleAsync(DataBaseContext context, string name)
    {
        // IsSystemAdmin is intentionally left false (default) — SQL-only per
        // design.md D1, never set from application code.
        var role = new RoleEntity { Name = name };
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    private static async Task GrantActionsAsync(DataBaseContext context, int roleId, IEnumerable<ActionEntity> actions)
    {
        foreach (var action in actions)
            context.RoleActions.Add(new RoleActionEntity { RoleId = roleId, ActionId = action.Id });

        await context.SaveChangesAsync();
    }

    private static async Task<UserEntity> CreateUserAsync(DataBaseContext context, string email, int roleId)
    {
        var user = new UserEntity
        {
            Email = email,
            PasswordHash = "not-a-real-hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            RoleId = roleId,
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private static async Task CleanupAsync(int roleId, IReadOnlyList<int> moduleIdsChildFirst, int userId)
    {
        await using var context = new DataBaseContext(BuildOptions());

        context.RoleActions.RemoveRange(context.RoleActions.Where(ra => ra.RoleId == roleId));
        await context.SaveChangesAsync();

        context.Users.RemoveRange(context.Users.Where(u => u.Id == userId));
        await context.SaveChangesAsync();

        context.Roles.RemoveRange(context.Roles.Where(r => r.Id == roleId));
        await context.SaveChangesAsync();

        foreach (var moduleId in moduleIdsChildFirst)
        {
            context.Actions.RemoveRange(context.Actions.Where(a => a.ModuleId == moduleId));
            await context.SaveChangesAsync();

            context.Modules.RemoveRange(context.Modules.Where(m => m.Id == moduleId));
            await context.SaveChangesAsync();
        }
    }

    private static void SkipUnlessRealDatabasesConfigured()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")))
        {
            throw new InvalidOperationException(
                "BLOCKED (infrastructure, not a test/code defect): this scenario requires a real, " +
                "migrated per-tenant SQL Server database. Set INVENTORY_TEST_DEFAULT_CONNECTION to run it. " +
                "See proposal.md Dependencies: \"the user will create the test tenant DB, or ask for it to be created.\"");
        }
    }
}
