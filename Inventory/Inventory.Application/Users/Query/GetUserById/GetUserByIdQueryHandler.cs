using Inventory.Application.Users.Dto;
using Inventory.Domain.Users.DomainUsers;
using MediatR;

namespace Inventory.Application.Users.Query.GetUserById;

public class GetUserByIdQueryHandler(IUserGetByIdService userGetByIdService)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userGetByIdService.GetByIdAsync(request.Id);
        return user is null ? null : UserDto.FromEntity(user);
    }
}
