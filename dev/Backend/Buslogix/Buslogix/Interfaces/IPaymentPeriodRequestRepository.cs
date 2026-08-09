using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodRequestRepository
    {
        Task<PaymentPeriodRequest?> GetPaymentPeriodRequest(int companyId, int id);
        Task<PagedResult<PaymentPeriodRequest>> GetPaymentPeriodRequests(int companyId, int page = 1, int pageSize = 20);
        Task<int> InsertPaymentPeriodRequest(int companyId, PaymentPeriodRequest request);
        Task<int> UpdatePaymentPeriodRequest(int companyId, int id, PaymentPeriodRequest request);
        Task<int> DeletePaymentPeriodRequest(int companyId, int id);
    }
}
