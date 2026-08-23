using Inventory.Application.Users.Dto;
using MediatR;

namespace Inventory.Application.Users.Query.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
