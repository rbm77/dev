using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentPeriodRequestRepository
    {
        Task<PaymentPeriodRequest?> GetPaymentPeriodRequest(int companyId, int id);
        Task<List<PaymentPeriodRequest>> GetPaymentPeriodRequests(int companyId);
        Task<int> InsertPaymentPeriodRequest(int companyId, PaymentPeriodRequest request);
        Task<int> UpdatePaymentPeriodRequest(int companyId, int id, PaymentPeriodRequest request);
        Task<int> DeletePaymentPeriodRequest(int companyId, int id);
    }
}
