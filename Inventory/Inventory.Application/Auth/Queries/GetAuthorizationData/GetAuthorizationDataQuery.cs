using Inventory.Domain.Common.Authorization;
using MediatR;

namespace Inventory.Application.Auth.Queries.GetAuthorizationData;

public record GetAuthorizationDataQuery(int UserId) : IRequest<UserAuthorizationData>;
