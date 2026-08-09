using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodRequestService
    {
        Task<PaymentPeriodRequest?> GetPaymentPeriodRequest(int companyId, int id);
        Task<PagedResult<PaymentPeriodRequest>> GetPaymentPeriodRequests(int companyId, int page = 1, int pageSize = 20);
        Task<int> InsertPaymentPeriodRequest(int companyId, PaymentPeriodRequest request);
        Task<bool> UpdatePaymentPeriodRequest(int companyId, int id, PaymentPeriodRequest request);
        Task<bool> DeletePaymentPeriodRequest(int companyId, int id);
    }
}
