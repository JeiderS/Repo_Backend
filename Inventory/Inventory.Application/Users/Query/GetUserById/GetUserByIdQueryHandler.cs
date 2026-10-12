using Inventory.Application.Users.Dto;
using Inventory.Application.Users.Errors;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Users.Query.GetUserById;

/// <summary>
/// Antes devolvía null y era el CONTROLLER el que decidía el mensaje "El
/// usuario indicado no existe". Ahora devuelve un Result como los commands:
/// el error y su mensaje los define Application, y el controller solo traduce.
/// </summary>
public class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        if (user is null)
            return UserErrorBuilder.UserNotFound();

        return UserDto.FromEntity(user);
    }
}
