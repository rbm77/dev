using Buslogix.Interfaces;
using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Services
{
    public class MaintenanceService(IMaintenanceRepository maintenanceRepository) : IMaintenanceService
    {

        private readonly IMaintenanceRepository _maintenanceRepository = maintenanceRepository;

        public async Task<Maintenance?> GetMaintenance(int companyId, int id)
        {
            return await _maintenanceRepository.GetMaintenance(companyId, id);
        }

        public async Task<List<Maintenance>> GetPendingMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        )
        {
            return await _maintenanceRepository.GetPendingMaintenances(companyId, vehicleId, type);
        }

        public async Task<List<Maintenance>> GetCompletedMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        )
        {
            return await _maintenanceRepository.GetCompletedMaintenances(companyId, vehicleId, type);
        }

        public async Task<int> InsertMaintenance(int companyId, Maintenance maintenance)
        {
            return await _maintenanceRepository.InsertMaintenance(companyId, maintenance);
        }

        public async Task<bool> UpdateMaintenance(int companyId, int id, Maintenance maintenance)
        {
            int affected = await _maintenanceRepository.UpdateMaintenance(companyId, id, maintenance);
            return affected > 0;
        }

        public async Task<bool> DeleteMaintenance(int companyId, int id)
        {
            int affected = await _maintenanceRepository.DeleteMaintenance(companyId, id);
            return affected > 0;
        }
    }
}