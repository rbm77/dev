using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodService
    {
        Task<PagedResult<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId, int page = 1, int pageSize = 20);
        Task<SchedulePaymentPeriodsResult> SchedulePaymentPeriods();
    }
}
