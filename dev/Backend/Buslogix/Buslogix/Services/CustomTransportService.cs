using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class CustomTransportService(ICustomTransportRepository customTransportRepository) : ICustomTransportService
    {

        public async Task<CustomTransport?> GetCustomTransport(int companyId, int id)
        {
            return await customTransportRepository.GetCustomTransport(companyId, id);
        }

        public async Task<List<CustomTransport>> GetPendingCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            return await customTransportRepository.GetPendingCustomTransports(companyId, vehicleId, driverId);
        }

        public async Task<List<CustomTransport>> GetCompletedCustomTransports(
            int companyId,
            int? vehicleId = null,
            int? driverId = null
        )
        {
            return await customTransportRepository.GetCompletedCustomTransports(companyId, vehicleId, driverId);
        }

        public async Task<int> InsertCustomTransport(int companyId, CustomTransport customTransport)
        {
            return await customTransportRepository.InsertCustomTransport(companyId, customTransport);
        }

        public async Task<bool> UpdateCustomTransport(int companyId, int id, CustomTransport customTransport)
        {
            int affected = await customTransportRepository.UpdateCustomTransport(companyId, id, customTransport);
            return affected > 0;
        }

        public async Task<bool> DeleteCustomTransport(int companyId, int id)
        {
            int affected = await customTransportRepository.DeleteCustomTransport(companyId, id);
            return affected > 0;
        }

        public async Task<bool> CompleteCustomTransport(int companyId, int id, CustomTransport customTransport)
        {
            int affected = await customTransportRepository.CompleteCustomTransport(companyId, id, customTransport);
            return affected > 0;
        }
    }
}