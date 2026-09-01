using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Services
{
    public class PaymentPeriodService(IPaymentPeriodRepository paymentPeriodRepository) : IPaymentPeriodService
    {

        public async Task<PagedResult<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId, int page = 1, int pageSize = 20)
        {
            return await paymentPeriodRepository.GetPaymentPeriods(companyId, requestId, page, pageSize);
        }

        public async Task<SchedulePaymentPeriodsResult> SchedulePaymentPeriods()
        {
            return await paymentPeriodRepository.SchedulePaymentPeriods();
        }
    }
}
