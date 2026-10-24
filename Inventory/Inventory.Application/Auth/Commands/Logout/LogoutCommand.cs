using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Auth.Commands.Logout;

public class LogoutCommand : IRequest<Result<VoidResult, Error>>
{
    /// <summary>Null si el cliente ya no tenía cookie — no es un error, el logout "funciona" igual.</summary>
    public string? RawToken { get; set; }
}
