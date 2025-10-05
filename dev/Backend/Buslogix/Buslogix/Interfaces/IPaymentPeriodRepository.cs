using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodRepository
    {
        Task<List<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId);
        Task<PaymentPeriod?> SchedulePaymentPeriod(string companyToken);
    }
}
