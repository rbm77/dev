using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class PaymentRepository(IDataAccess dataAccess) : IPaymentRepository
    {

        public async Task<Payment?> GetPayment(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Payment> rows = await dataAccess.ExecuteReader("get_payment", CommandType.StoredProcedure,
                static reader => new Payment
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    StudentId = reader.GetInt32OrDefault(3)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Payment>> GetPayments(
            int companyId,
            DateTime? date = null,
            int? studentId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = date,
                ["p_student_id"] = studentId
            };

            List<Payment> rows = await dataAccess.ExecuteReader("get_payments", CommandType.StoredProcedure,
                static reader => new Payment
                {
                    Id = reader.GetInt64OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    StudentId = reader.GetInt32OrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<long> InsertPayment(int companyId, Payment payment)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = payment.Date,
                ["p_amount"] = payment.Amount,
                ["p_student_id"] = payment.StudentId
            };

            object? result = await dataAccess.ExecuteScalar("insert_payment", CommandType.StoredProcedure, parameters);
            return result != null ? (long)result : 0L;
        }

        public async Task<int> UpdatePayment(int companyId, long id, Payment payment)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_date"] = payment.Date,
                ["p_amount"] = payment.Amount,
                ["p_student_id"] = payment.StudentId
            };

            return await dataAccess.ExecuteNonQuery("update_payment", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeletePayment(int companyId, long id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_payment", CommandType.StoredProcedure, parameters);
        }
    }
}