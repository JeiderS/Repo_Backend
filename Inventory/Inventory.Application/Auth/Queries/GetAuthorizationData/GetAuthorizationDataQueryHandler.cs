using Inventory.Domain.Common.Authorization;
using Inventory.Domain.Users.DomainUsers;
using MediatR;

namespace Inventory.Application.Auth.Queries.GetAuthorizationData;

public class GetAuthorizationDataQueryHandler(IUserAuthorizationService userAuthorizationService)
    : IRequestHandler<GetAuthorizationDataQuery, UserAuthorizationData>
{
    public Task<UserAuthorizationData> Handle(GetAuthorizationDataQuery request, CancellationToken cancellationToken)
    {
        return userAuthorizationService.GetAuthorizationDataAsync(request.UserId);
    }
}
