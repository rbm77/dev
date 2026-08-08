using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IReportRepository
    {
        Task<PagedResult<Debtor>> GetDebtors(int companyId, int? routeId, int? studentId, bool? isActive, int page, int pageSize);
    }
}
