using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Interfaces
{
    public interface IIncidentRepository
    {
        Task<Incident?> GetIncident(int companyId, int id);
        Task<List<Incident>> GetIncidents(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            IncidentType? type = null
        );
        Task<int> InsertIncident(int companyId, Incident incident);
        Task<int> UpdateIncident(int companyId, int id, Incident incident);
        Task<int> DeleteIncident(int companyId, int id);
    }
}