using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISpecificExemptionService
    {
        Task<SpecificExemption?> GetSpecificExemption(int companyId, int id);

        Task<List<SpecificExemption>> GetSpecificExemptions(
            int companyId,
            int? studentId = null,
            int? paymentPeriodId = null
        );

        Task<int> InsertSpecificExemption(int companyId, SpecificExemption exemption);

        Task<bool> UpdateSpecificExemption(int companyId, int id, SpecificExemption exemption);

        Task<bool> DeleteSpecificExemption(int companyId, int id);
    }
}