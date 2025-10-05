using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class PaymentPeriodRepository(IDataAccess dataAccess) : IPaymentPeriodRepository
    {

        public async Task<List<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_request_id"] = requestId
            };

            List<PaymentPeriod> rows = await dataAccess.ExecuteReader("get_payment_periods", CommandType.StoredProcedure,
                static reader => new PaymentPeriod
                {
                    Id = reader.GetInt32OrDefault(0),
                    RequestId = reader.GetInt32OrDefault(1),
                    PaymentDate = reader.GetDateTimeOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<PaymentPeriod?> SchedulePaymentPeriod(string companyToken)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_token"] = companyToken
            };

            List<PaymentPeriod> rows = await dataAccess.ExecuteReader("schedule_payment_period", CommandType.StoredProcedure,
                static reader => new PaymentPeriod
                {
                    Id = reader.GetInt32OrDefault(0),
                    RequestId = reader.GetInt32OrDefault(1),
                    PaymentDate = reader.GetDateTimeOrDefault(2)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }
    }
}