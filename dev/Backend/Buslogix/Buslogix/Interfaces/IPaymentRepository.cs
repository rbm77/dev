using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPayment(int companyId, long id);

        Task<List<Payment>> GetPayments(
            int companyId,
            DateTime? date = null,
            int? studentId = null
        );

        Task<long> InsertPayment(int companyId, Payment payment);

        Task<int> UpdatePayment(int companyId, long id, Payment payment);

        Task<int> DeletePayment(int companyId, long id);
    }
}