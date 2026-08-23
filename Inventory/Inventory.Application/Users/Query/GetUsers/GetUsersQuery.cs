using Inventory.Application.Users.Dto;
using MediatR;

namespace Inventory.Application.Users.Query.GetUsers;

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
