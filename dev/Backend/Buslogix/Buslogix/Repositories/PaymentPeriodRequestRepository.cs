using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class PaymentPeriodRequestRepository(IDataAccess dataAccess) : IPaymentPeriodRequestRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<PaymentPeriodRequest?> GetPaymentPeriodRequest(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<PaymentPeriodRequest> rows = await _dataAccess.ExecuteReader("get_payment_period_request", CommandType.StoredProcedure,
                static reader => new PaymentPeriodRequest
                {
                    Id = reader.GetInt32OrDefault(0),
                    Amount = reader.GetDecimalOrDefault(1),
                    StartDate = reader.GetDateTimeOrDefault(2),
                    DaysToNextPayment = reader.GetInt32OrDefault(3)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<PaymentPeriodRequest>> GetPaymentPeriodRequests(int companyId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId
            };

            List<PaymentPeriodRequest> rows = await _dataAccess.ExecuteReader("get_payment_period_requests", CommandType.StoredProcedure,
                static reader => new PaymentPeriodRequest
                {
                    Id = reader.GetInt32OrDefault(0),
                    StartDate = reader.GetDateTimeOrDefault(1),
                }, parameters);

            return rows;
        }

        public async Task<int> InsertPaymentPeriodRequest(int companyId, PaymentPeriodRequest request)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_amount"] = request.Amount,
                ["p_start_date"] = request.StartDate,
                ["p_days_to_next_payment"] = request.DaysToNextPayment
            };

            object? result = await _dataAccess.ExecuteScalar("insert_payment_period_request", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdatePaymentPeriodRequest(int companyId, int id, PaymentPeriodRequest request)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_amount"] = request.Amount,
                ["p_start_date"] = request.StartDate,
                ["p_days_to_next_payment"] = request.DaysToNextPayment
            };

            return await _dataAccess.ExecuteNonQuery("update_payment_period_request", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeletePaymentPeriodRequest(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_payment_period_request", CommandType.StoredProcedure, parameters);
        }
    }
}