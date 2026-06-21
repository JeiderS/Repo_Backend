namespace Inventory.Application.Auth.Dto;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? FullName { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
