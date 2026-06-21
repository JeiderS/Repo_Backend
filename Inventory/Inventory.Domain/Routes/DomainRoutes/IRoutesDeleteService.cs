
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;

namespace Inventory.Domain.Routes.DomainRoutes
{
    public interface IRoutesDeleteService
    {
        Task<Result<VoidResult, Error>> DeleteAsync(int id);
    }
}
