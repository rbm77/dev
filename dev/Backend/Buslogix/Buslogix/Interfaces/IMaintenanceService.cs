using Buslogix.Models;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Interfaces
{
    public interface IMaintenanceService
    {
        Task<Maintenance?> GetMaintenance(int companyId, int id);

        Task<PagedResult<Maintenance>> GetPendingMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null,
            int page = 1,
            int pageSize = 20
        );

        Task<PagedResult<Maintenance>> GetCompletedMaintenances(
            int companyId,
            int? vehicleId = null,
            MaintenanceType? type = null,
            int page = 1,
            int pageSize = 20
        );

        Task<int> InsertMaintenance(int companyId, Maintenance maintenance);

        Task<bool> UpdateMaintenance(int companyId, int id, Maintenance maintenance);

        Task<bool> DeleteMaintenance(int companyId, int id);

        Task<bool> CompleteMaintenance(int companyId, int id, Maintenance maintenance);
    }
}
