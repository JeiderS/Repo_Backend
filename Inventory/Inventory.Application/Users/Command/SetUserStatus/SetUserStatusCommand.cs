using Inventory.Application.Users.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Users.Command.SetUserStatus;

public class SetUserStatusCommand : IRequest<Result<UserDto, Error>>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
