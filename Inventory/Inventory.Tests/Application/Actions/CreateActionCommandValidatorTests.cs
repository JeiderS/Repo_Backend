using Inventory.Application.Actions.Command.CreateAction;
using Xunit;

namespace Inventory.Tests.Application.Actions;

/// <summary>
/// Cubre la regla de negocio "el código debe ser válido" que antes vivía en
/// ActionEntity.Create (ver ActionEntityTests) y que el commit d12fd7a movió
/// aquí: ahora Code lo provee quien llama, así que el validador es quien
/// decide si es aceptable.
/// </summary>
public class CreateActionCommandValidatorTests
{
    private readonly CreateActionCommandValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123View")]
    [InlineData("Users View")]
    [InlineData("Users-View")]
    [InlineData(" UsersView")]
    [InlineData("UsersView ")]
    public void Validate_InvalidCode_IsRejected(string code)
    {
        var result = _validator.Validate(new CreateActionCommand(code, "View"));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("UsersView")]
    [InlineData("InventoryCreate")]
    [InlineData("A")]
    public void Validate_ValidCode_IsAccepted(string code)
    {
        var result = _validator.Validate(new CreateActionCommand(code, "View"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyName_IsRejected()
    {
        var result = _validator.Validate(new CreateActionCommand("UsersView", ""));

        Assert.False(result.IsValid);
    }
}
