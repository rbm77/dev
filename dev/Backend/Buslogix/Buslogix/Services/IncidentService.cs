using Buslogix.Interfaces;
using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Services
{
    public class IncidentService(IIncidentRepository incidentRepository) : IIncidentService
    {

        public async Task<Incident?> GetIncident(int companyId, int id)
        {
            return await incidentRepository.GetIncident(companyId, id);
        }

        public async Task<List<Incident>> GetIncidents(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            IncidentType? type = null
        )
        {
            return await incidentRepository.GetIncidents(companyId, vehicleId, driverId, type);
        }

        public async Task<int> InsertIncident(int companyId, Incident incident)
        {
            return await incidentRepository.InsertIncident(companyId, incident);
        }

        public async Task<bool> UpdateIncident(int companyId, int id, Incident incident)
        {
            int affected = await incidentRepository.UpdateIncident(companyId, id, incident);
            return affected > 0;
        }

        public async Task<bool> DeleteIncident(int companyId, int id)
        {
            int affected = await incidentRepository.DeleteIncident(companyId, id);
            return affected > 0;
        }
    }
}