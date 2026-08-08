using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class ReportService(IReportRepository reportRepository) : IReportService
    {
        public async Task<PagedResult<Debtor>> GetDebtors(int companyId, int? routeId, int? studentId, bool? isActive, int page, int pageSize)
        {
            return await reportRepository.GetDebtors(companyId, routeId, studentId, isActive, page, pageSize);
        }
    }
}
