using Inventory.Application.Users.Dto;
using Inventory.Domain.Users.Repositories;
using MediatR;

namespace Inventory.Application.Users.Query.GetUserById;

public class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        return user is null ? null : UserDto.FromEntity(user);
    }
}
