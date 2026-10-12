using Inventory.Application.Users.Dto;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Users.Query.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(UserDto.FromEntity).ToList();
    }
}
