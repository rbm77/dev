
namespace Buslogix.Features.Reports
{
    public interface IReportRepository
    {
        Task<PagedResult<Debtor>> GetDebtors(int companyId, int? routeId, int? studentId, bool? isActive, int page, int pageSize);
    }
}
