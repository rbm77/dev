using Buslogix.Interfaces;
using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Services
{
    public class IncidentService(IIncidentRepository incidentRepository) : IIncidentService
    {

        private readonly IIncidentRepository _incidentRepository = incidentRepository;

        public async Task<Incident?> GetIncident(int companyId, int id)
        {
            return await _incidentRepository.GetIncident(companyId, id);
        }

        public async Task<List<Incident>> GetIncidents(
            int companyId,
            int? vehicleId = null,
            int? driverId = null,
            IncidentType? type = null
        )
        {
            return await _incidentRepository.GetIncidents(companyId, vehicleId, driverId, type);
        }

        public async Task<int> InsertIncident(int companyId, Incident incident)
        {
            return await _incidentRepository.InsertIncident(companyId, incident);
        }

        public async Task<bool> UpdateIncident(int companyId, int id, Incident incident)
        {
            int affected = await _incidentRepository.UpdateIncident(companyId, id, incident);
            return affected > 0;
        }

        public async Task<bool> DeleteIncident(int companyId, int id)
        {
            int affected = await _incidentRepository.DeleteIncident(companyId, id);
            return affected > 0;
        }
    }
}