using Buslogix.Interfaces;
using Buslogix.Matching;
using Buslogix.Matching.Abstractions;
using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Services
{
    public class PaymentRequestService(
        IPaymentRequestRepository paymentRequestRepository,
        IPaymentMatchQueue paymentMatchQueue) : IPaymentRequestService
    {

        public async Task<PaymentRequest?> GetPaymentRequest(int companyId, int id)
        {
            return await paymentRequestRepository.GetPaymentRequest(companyId, id);
        }

        public async Task<PagedResult<PaymentRequest>> GetPaymentRequests(int companyId, DateTime? requestedAt = null, bool? isValidated = null, string? receiptReference = null, int page = 1, int pageSize = 20)
        {
            return await paymentRequestRepository.GetPaymentRequests(companyId, requestedAt, isValidated, receiptReference, page, pageSize);
        }

        public async Task<int> InsertPaymentRequest(int companyId, PaymentRequest paymentRequest)
        {
            int id = await paymentRequestRepository.InsertPaymentRequest(companyId, paymentRequest);

            if (id > 0)
            {
                // Inserted successfully - try to pair it against a pending
                // message_extraction_result in the background, without
                // holding up this request.
                await paymentMatchQueue.EnqueueAsync(new PaymentMatchRequest(companyId, paymentRequest.ReceiptReference));
            }

            return id;
        }

        public async Task<long> ApprovePaymentRequest(int companyId, int id)
        {
            return await paymentRequestRepository.ApprovePaymentRequest(companyId, id);
        }

        public async Task<bool> RejectPaymentRequest(int companyId, int id)
        {
            return await paymentRequestRepository.RejectPaymentRequest(companyId, id);
        }

        public async Task<bool> ValidatePaymentRequest(int companyId, int id)
        {
            return await paymentRequestRepository.ValidatePaymentRequest(companyId, id);
        }

        public async Task<AutoApprovalResult> AutoApprovePaymentRequests()
        {
            return await paymentRequestRepository.AutoApprovePaymentRequests();
        }
    }
}
