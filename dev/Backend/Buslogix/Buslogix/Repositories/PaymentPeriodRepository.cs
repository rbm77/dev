using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class PaymentPeriodRepository(IDataAccess dataAccess) : IPaymentPeriodRepository
    {

        public async Task<PagedResult<PaymentPeriod>> GetPaymentPeriods(int companyId, int? requestId, int page = 1, int pageSize = 20)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_request_id"] = requestId,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<PaymentPeriod> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_payment_periods", CommandType.StoredProcedure,
                static reader => new PaymentPeriod
                {
                    Id = reader.GetInt32OrDefault(0),
                    RequestId = reader.GetInt32OrDefault(1),
                    PaymentDate = reader.GetDateTimeOrDefault(2)
                }, parameters);

            return new PagedResult<PaymentPeriod>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
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