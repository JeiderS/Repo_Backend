using Inventory.Application.Users.Dto;
using Inventory.Domain.Users.DomainUsers;
using MediatR;

namespace Inventory.Application.Users.Query.GetUsers;

public class GetUsersQueryHandler(IUserListService userListService)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userListService.GetAllAsync();
        return users.Select(UserDto.FromEntity).ToList();
    }
}
