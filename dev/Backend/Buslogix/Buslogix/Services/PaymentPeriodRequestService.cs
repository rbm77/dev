using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class PaymentPeriodRequestService(IPaymentPeriodRequestRepository paymentPeriodRequestRepository) : IPaymentPeriodRequestService
    {

        private readonly IPaymentPeriodRequestRepository _paymentPeriodRequestRepository = paymentPeriodRequestRepository;

        public async Task<PaymentPeriodRequest?> GetPaymentPeriodRequest(int companyId, int id)
        {
            return await _paymentPeriodRequestRepository.GetPaymentPeriodRequest(companyId, id);
        }

        public async Task<List<PaymentPeriodRequest>> GetPaymentPeriodRequests(int companyId)
        {
            return await _paymentPeriodRequestRepository.GetPaymentPeriodRequests(companyId);
        }

        public async Task<int> InsertPaymentPeriodRequest(int companyId, PaymentPeriodRequest request)
        {
            return await _paymentPeriodRequestRepository.InsertPaymentPeriodRequest(companyId, request);
        }

        public async Task<bool> UpdatePaymentPeriodRequest(int companyId, int id, PaymentPeriodRequest request)
        {
            int affected = await _paymentPeriodRequestRepository.UpdatePaymentPeriodRequest(companyId, id, request);
            return affected > 0;
        }

        public async Task<bool> DeletePaymentPeriodRequest(int companyId, int id)
        {
            int affected = await _paymentPeriodRequestRepository.DeletePaymentPeriodRequest(companyId, id);
            return affected > 0;
        }
    }
}