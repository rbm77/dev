using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using System.Data;

namespace Buslogix.Repositories
{
    public class ReportRepository(IDataAccess dataAccess) : IReportRepository
    {

        public async Task<PagedResult<Debtor>> GetDebtors(int companyId, int? routeId, int? studentId, bool? isActive, int page, int pageSize)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_route_id"] = routeId,
                ["p_student_id"] = studentId,
                ["p_page"] = page,
                ["p_page_size"] = pageSize,
                ["p_is_active"] = isActive
            };

            (List<Debtor> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_debtors", CommandType.StoredProcedure,
                static reader => new Debtor
                {
                    Id = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1) ?? "",
                    LastName = reader.GetStringOrDefault(2) ?? "",
                    IdentityDocument = reader.GetStringOrDefault(3),
                    GradeId = reader.GetInt32OrDefault(4),
                    RouteId = reader.GetInt32OrDefault(5),
                    EntryDate = reader.GetDateTimeOrDefault(6) ?? default,
                    IsActive = reader.GetBooleanOrDefault(7),
                    DueAmount = reader.GetDecimalOrDefault(8),
                    PeriodsCount = reader.GetInt32OrDefault(9),
                    PaymentsCount = reader.GetInt32OrDefault(10)
                }, parameters);

            return new PagedResult<Debtor>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
