using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Interfaces
{
    public interface IIncidentService
    {
        Task<Incident?> GetIncident(int companyId, int id);
        Task<PagedResult<Incident>> GetIncidents(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            IncidentType? type = null,
            int page = 1,
            int pageSize = 20
        );
        Task<int> InsertIncident(int companyId, Incident incident);
        Task<bool> UpdateIncident(int companyId, int id, Incident incident);
        Task<bool> DeleteIncident(int companyId, int id);
    }
}
