using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IPaymentRequestService
    {
        Task<PaymentRequest?> GetPaymentRequest(int companyId, int id);

        Task<PagedResult<PaymentRequest>> GetPaymentRequests(int companyId, DateTime? requestedAt = null, bool? isValidated = null, string? receiptReference = null, int page = 1, int pageSize = 20);

        Task<int> InsertPaymentRequest(int companyId, PaymentRequest paymentRequest);

        Task<long> ApprovePaymentRequest(int companyId, int id);

        Task<bool> RejectPaymentRequest(int companyId, int id);

        Task<bool> ValidatePaymentRequest(int companyId, int id);

        Task<AutoApprovalResult> AutoApprovePaymentRequests();
    }
}
