using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodRequestService
    {
        Task<PaymentPeriodRequest?> GetPaymentPeriodRequest(int companyId, int id);
        Task<List<PaymentPeriodRequest>> GetPaymentPeriodRequests(int companyId);
        Task<int> InsertPaymentPeriodRequest(int companyId, PaymentPeriodRequest request);
        Task<bool> UpdatePaymentPeriodRequest(int companyId, int id, PaymentPeriodRequest request);
        Task<bool> DeletePaymentPeriodRequest(int companyId, int id);
    }
}
