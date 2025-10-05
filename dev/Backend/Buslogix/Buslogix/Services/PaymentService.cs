using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
    {

        public async Task<Payment?> GetPayment(int companyId, long id)
        {
            return await paymentRepository.GetPayment(companyId, id);
        }

        public async Task<List<Payment>> GetPayments(
            int companyId,
            DateTime? date = null,
            int? studentId = null
        )
        {
            return await paymentRepository.GetPayments(companyId, date, studentId);
        }

        public async Task<long> InsertPayment(int companyId, Payment payment)
        {
            return await paymentRepository.InsertPayment(companyId, payment);
        }

        public async Task<bool> UpdatePayment(int companyId, long id, Payment payment)
        {
            int affected = await paymentRepository.UpdatePayment(companyId, id, payment);
            return affected > 0;
        }

        public async Task<bool> DeletePayment(int companyId, long id)
        {
            int affected = await paymentRepository.DeletePayment(companyId, id);
            return affected > 0;
        }
    }
}