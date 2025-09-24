using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ICustomTransportRepository
    {
        Task<CustomTransport?> GetCustomTransport(int companyId, int id);

        Task<List<CustomTransport>> GetPendingCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null
        );

        Task<List<CustomTransport>> GetCompletedCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null
        );

        Task<int> InsertCustomTransport(int companyId, CustomTransport customTransport);

        Task<int> UpdateCustomTransport(int companyId, int id, CustomTransport customTransport);

        Task<int> DeleteCustomTransport(int companyId, int id);

        Task<int> CompleteCustomTransport(int companyId, int id, CustomTransport customTransport);
    }
}
