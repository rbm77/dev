
namespace Buslogix.Features.PaymentPeriods
{
    public interface IPaymentPeriodRepository
    {
        Task<PagedResult<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId, int page = 1, int pageSize = 20);
        Task<SchedulePaymentPeriodsResult> SchedulePaymentPeriods();
    }
}
