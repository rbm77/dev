using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface ISpecificExemptionRepository
    {
        Task<SpecificExemption?> GetSpecificExemption(int companyId, int id);

        Task<List<SpecificExemption>> GetSpecificExemptions(
            int companyId,
            int? studentId = null,
            int? paymentPeriodId = null
        );

        Task<int> InsertSpecificExemption(int companyId, SpecificExemption exemption);

        Task<int> UpdateSpecificExemption(int companyId, int id, SpecificExemption exemption);

        Task<int> DeleteSpecificExemption(int companyId, int id);
    }
}