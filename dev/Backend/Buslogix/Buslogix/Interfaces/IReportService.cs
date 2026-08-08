using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IReportService
    {
        Task<PagedResult<Debtor>> GetDebtors(int companyId, int? routeId, int? studentId, bool? isActive, int page, int pageSize);
    }
}
