using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ICustomTransportRepository
    {
        Task<CustomTransport?> GetCustomTransport(int companyId, int id);

        Task<PagedResult<CustomTransport>> GetPendingCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            int page = 1,
            int pageSize = 20
        );

        Task<PagedResult<CustomTransport>> GetCompletedCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            int page = 1,
            int pageSize = 20
        );

        Task<int> InsertCustomTransport(int companyId, CustomTransport customTransport);

        Task<int> UpdateCustomTransport(int companyId, int id, CustomTransport customTransport);

        Task<int> DeleteCustomTransport(int companyId, int id);

        Task<int> CompleteCustomTransport(int companyId, int id, CustomTransport customTransport);
    }
}
