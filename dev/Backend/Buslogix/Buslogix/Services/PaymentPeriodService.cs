using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class PaymentPeriodService(IPaymentPeriodRepository paymentPeriodRepository) : IPaymentPeriodService
    {

        public async Task<List<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId)
        {
            return await paymentPeriodRepository.GetPaymentPeriods(companyId, requestId);
        }

        public async Task<PaymentPeriod?> SchedulePaymentPeriod(string companyToken)
        {
            return await paymentPeriodRepository.SchedulePaymentPeriod(companyToken);
        }
    }
}
