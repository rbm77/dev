using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodService
    {
        Task<List<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId);
        Task<PaymentPeriod?> SchedulePaymentPeriod(string companyToken);
    }
}
