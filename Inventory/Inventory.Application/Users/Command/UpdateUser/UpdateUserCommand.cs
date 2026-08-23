using Inventory.Application.Users.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Users.Command.UpdateUser;

public class UpdateUserCommand : IRequest<Result<UserDto, Error>>
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public int? RoleId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
