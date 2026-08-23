using Inventory.Application.Users.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Users.Command.CreateUser;

public class CreateUserCommand : IRequest<Result<UserDto, Error>>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool? MustChangePassword { get; set; }
    public int? RoleId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
