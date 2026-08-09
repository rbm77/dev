using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class SpecificExemptionService(ISpecificExemptionRepository specificExemptionRepository) : ISpecificExemptionService
    {

        public async Task<SpecificExemption?> GetSpecificExemption(int companyId, int id)
        {
            return await specificExemptionRepository.GetSpecificExemption(companyId, id);
        }

        public async Task<PagedResult<SpecificExemption>> GetSpecificExemptions(
            int companyId,
            int? studentId,
            int? paymentPeriodId,
            int page,
            int pageSize
        )
        {
            return await specificExemptionRepository.GetSpecificExemptions(companyId, studentId, paymentPeriodId, page, pageSize);
        }

        public async Task<int> InsertSpecificExemption(int companyId, SpecificExemption exemption)
        {
            return await specificExemptionRepository.InsertSpecificExemption(companyId, exemption);
        }

        public async Task<bool> UpdateSpecificExemption(int companyId, int id, SpecificExemption exemption)
        {
            int affected = await specificExemptionRepository.UpdateSpecificExemption(companyId, id, exemption);
            return affected > 0;
        }

        public async Task<bool> DeleteSpecificExemption(int companyId, int id)
        {
            int affected = await specificExemptionRepository.DeleteSpecificExemption(companyId, id);
            return affected > 0;
        }
    }
}