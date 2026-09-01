using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
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

        public async Task<SchedulePaymentPeriodsResult> SchedulePaymentPeriods()
        {
            List<SchedulePaymentPeriodsResult> rows = await dataAccess.ExecuteReader("schedule_payment_periods", CommandType.StoredProcedure,
                static reader => new SchedulePaymentPeriodsResult
                {
                    ProcessedCount = reader.GetInt32OrDefault(0),
                    ScheduledCount = reader.GetInt32OrDefault(1),
                    SkippedCount = reader.GetInt32OrDefault(2),
                    FailedCount = reader.GetInt32OrDefault(3)
                }, null);

            return rows.Count > 0 ? rows[0] : new SchedulePaymentPeriodsResult();
        }
    }
}