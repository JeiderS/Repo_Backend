using Inventory.Domain.Users.Entity;

namespace Inventory.Application.Users.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public static UserDto FromEntity(UserEntity user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        IsActive = user.IsActive,
        MustChangePassword = user.MustChangePassword,
        RoleId = user.RoleId,
        RoleName = user.Role?.Name,
        FirstName = user.Profile?.FirstName,
        LastName = user.Profile?.LastName,
        Phone = user.Profile?.Phone,
        Address = user.Profile?.Address
    };
}
