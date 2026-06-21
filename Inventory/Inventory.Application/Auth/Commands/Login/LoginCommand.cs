using Inventory.Application.Auth.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<Result<AuthResponseDto, Error>>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
