namespace Inventory.Domain.Common.Results.Errors;

/// <summary>
/// Un error esperado del negocio (no una excepción). Es inmutable: una vez
/// creado no cambia. Se crea siempre con <see cref="CreateInstance"/>
/// (patrón Factory Method) desde los ErrorBuilder de cada módulo.
/// </summary>
public sealed class Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static Error CreateInstance(string code, string description, ErrorType type) =>
        new(code, description, type);
}
