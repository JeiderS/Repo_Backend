using System.Text.Json.Serialization;

namespace Inventory.Application.Auth.Dto;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? FullName { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();

    // Cruza la frontera MediatR -> Controller en C#, pero JAMÁS se serializa
    // en el body JSON: el valor crudo solo debe viajar en la cookie httpOnly
    // que setea el controller. Si también fuera al JSON, un XSS que lea la
    // respuesta se lo llevaría igual que si no fuera httpOnly.
    [JsonIgnore]
    public string RefreshToken { get; set; } = default!;
}
