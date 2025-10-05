using Buslogix.Interfaces;
using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Services
{
    public class MaintenanceService(IMaintenanceRepository maintenanceRepository) : IMaintenanceService
    {

        public async Task<Maintenance?> GetMaintenance(int companyId, int id)
        {
            return await maintenanceRepository.GetMaintenance(companyId, id);
        }

        public async Task<List<Maintenance>> GetPendingMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        )
        {
            return await maintenanceRepository.GetPendingMaintenances(companyId, vehicleId, type);
        }

        public async Task<List<Maintenance>> GetCompletedMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null
        )
        {
            return await maintenanceRepository.GetCompletedMaintenances(companyId, vehicleId, type);
        }

        public async Task<int> InsertMaintenance(int companyId, Maintenance maintenance)
        {
            return await maintenanceRepository.InsertMaintenance(companyId, maintenance);
        }

        public async Task<bool> UpdateMaintenance(int companyId, int id, Maintenance maintenance)
        {
            int affected = await maintenanceRepository.UpdateMaintenance(companyId, id, maintenance);
            return affected > 0;
        }

        public async Task<bool> DeleteMaintenance(int companyId, int id)
        {
            int affected = await maintenanceRepository.DeleteMaintenance(companyId, id);
            return affected > 0;
        }

        public async Task<bool> CompleteMaintenance(int companyId, int id, Maintenance maintenance)
        {
            int affected = await maintenanceRepository.CompleteMaintenance(companyId, id, maintenance);
            return affected > 0;
        }
    }
}