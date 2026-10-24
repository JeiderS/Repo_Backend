using Inventory.Application.Auth.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<AuthResponseDto, Error>>
{
    public string RawToken { get; set; } = default!;
}
