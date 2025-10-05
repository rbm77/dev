using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPayment(int companyId, long id);

        Task<List<Payment>> GetPayments(
            int companyId,
            DateTime? date = null,
            int? studentId = null
        );

        Task<long> InsertPayment(int companyId, Payment payment);

        Task<bool> UpdatePayment(int companyId, long id, Payment payment);

        Task<bool> DeletePayment(int companyId, long id);
    }
}