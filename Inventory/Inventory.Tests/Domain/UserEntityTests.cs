using Inventory.Domain.Roles.Entity;
using Inventory.Domain.UserProfile.Entity;
using Inventory.Domain.Users.Entity;
using Xunit;

namespace Inventory.Tests.Domain;

/// <summary>
/// Pruebas de las reglas de negocio del usuario. No necesitan base de datos,
/// ni API, ni nada externo: esa es la gran ventaja de que las reglas vivan
/// dentro de la entidad.
/// </summary>
public class UserEntityTests
{
    private static UserEntity NewUser(bool? mustChangePassword = null, RoleEntity? role = null) =>
        UserEntity.Create(
            "ana@example.com",
            "hash",
            UserProfileEntity.Create("Ana", "Pérez", phone: null, address: null),
            role,
            mustChangePassword,
            DateTime.UtcNow);

    [Fact]
    public void Create_NewUser_StartsActive_AndCanLogin()
    {
        var user = NewUser();

        Assert.True(user.IsActive);
        Assert.True(user.CanLogin());
    }

    [Fact]
    public void Create_WithoutSayingOtherwise_MustChangePassword()
    {
        Assert.True(NewUser().MustChangePassword);
        Assert.False(NewUser(mustChangePassword: false).MustChangePassword);
    }

    [Fact]
    public void Deactivate_InactiveUser_CannotLogin()
    {
        var user = NewUser();

        user.Deactivate();

        Assert.False(user.CanLogin());
    }

    [Fact]
    public void FullName_IsFirstNamePlusLastName()
    {
        Assert.Equal("Ana Pérez", NewUser().FullName);
    }

    [Fact]
    public void AssignRole_ReplacesPreviousRole_AndReportsTheChange()
    {
        var user = NewUser(role: null);
        var role = RoleEntity.Create("Ventas");

        Assert.False(user.AssignRole(null));  // sin rol -> sin rol: no cambió
        Assert.True(user.AssignRole(role));   // sin rol -> Ventas: cambió
        Assert.False(user.AssignRole(role));  // Ventas -> Ventas: no cambió
        Assert.Same(role, user.Role);
    }

    [Fact]
    public void RegisterFailedLogin_FiveConsecutiveTimes_LocksTheAccount()
    {
        var user = NewUser();
        var nowUtc = DateTime.UtcNow;

        for (var i = 0; i < 5; i++)
            user.RegisterFailedLogin(nowUtc);

        Assert.True(user.IsLockedOut(nowUtc));
        Assert.Equal(0, user.FailedLoginAttempts); // se reinicia al bloquear
    }

    [Fact]
    public void RegisterFailedLogin_FewerThanFiveTimes_DoesNotLockTheAccount()
    {
        var user = NewUser();
        var nowUtc = DateTime.UtcNow;

        for (var i = 0; i < 4; i++)
            user.RegisterFailedLogin(nowUtc);

        Assert.False(user.IsLockedOut(nowUtc));
        Assert.Equal(4, user.FailedLoginAttempts);
    }

    [Fact]
    public void RegisterSuccessfulLogin_ResetsFailedAttempts_AndLiftsAnyLockout()
    {
        var user = NewUser();
        var nowUtc = DateTime.UtcNow;

        for (var i = 0; i < 5; i++)
            user.RegisterFailedLogin(nowUtc);
        Assert.True(user.IsLockedOut(nowUtc));

        user.RegisterSuccessfulLogin();

        Assert.False(user.IsLockedOut(nowUtc));
        Assert.Equal(0, user.FailedLoginAttempts);
    }

    [Fact]
    public void IsLockedOut_AfterLockoutWindowPasses_ReturnsFalse()
    {
        var user = NewUser();
        var lockedAt = DateTime.UtcNow;

        for (var i = 0; i < 5; i++)
            user.RegisterFailedLogin(lockedAt);

        Assert.True(user.IsLockedOut(lockedAt.AddMinutes(14)));
        Assert.False(user.IsLockedOut(lockedAt.AddMinutes(15).AddSeconds(1)));
    }
}
