using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPeriodicExemptionService
    {
        Task<PeriodicExemption?> GetPeriodicExemption(int companyId, int id);

        Task<PagedResult<PeriodicExemption>> GetPeriodicExemptions(
            int companyId,
            int? studentId = null,
            int page = 1,
            int pageSize = 20
        );

        Task<int> InsertPeriodicExemption(int companyId, PeriodicExemption exemption);

        Task<bool> UpdatePeriodicExemption(int companyId, int id, PeriodicExemption exemption);

        Task<bool> DeletePeriodicExemption(int companyId, int id);
    }
}