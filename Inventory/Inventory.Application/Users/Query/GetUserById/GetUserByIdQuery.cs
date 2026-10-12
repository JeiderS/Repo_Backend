using Inventory.Application.Users.Dto;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using MediatR;

namespace Inventory.Application.Users.Query.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserDto, Error>>;
