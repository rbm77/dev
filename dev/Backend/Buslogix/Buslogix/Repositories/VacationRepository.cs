using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class VacationRepository(IDataAccess dataAccess) : IVacationRepository
    {

        public async Task<Vacation?> GetVacation(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Vacation> rows = await dataAccess.ExecuteReader("get_vacation", CommandType.StoredProcedure,
                static reader => new Vacation
                {
                    Id = reader.GetInt32OrDefault(0),
                    Description = reader.GetStringOrDefault(1),
                    StartDate = reader.GetDateTimeOrDefault(2),
                    EndDate = reader.GetDateTimeOrDefault(3)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<PagedResult<Vacation>> GetAllVacation(int companyId, int page, int pageSize)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<Vacation> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_all_vacation", CommandType.StoredProcedure,
                static reader => new Vacation
                {
                    Id = reader.GetInt32OrDefault(0),
                    StartDate = reader.GetDateTimeOrDefault(1),
                    EndDate = reader.GetDateTimeOrDefault(2)
                }, parameters);

            return new PagedResult<Vacation>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> InsertVacation(int companyId, Vacation vacation)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_description"] = vacation.Description,
                ["p_start_date"] = vacation.StartDate,
                ["p_end_date"] = vacation.EndDate
            };

            object? result = await dataAccess.ExecuteScalar("insert_vacation", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateVacation(int companyId, int id, Vacation vacation)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_description"] = vacation.Description,
                ["p_start_date"] = vacation.StartDate,
                ["p_end_date"] = vacation.EndDate
            };

            return await dataAccess.ExecuteNonQuery("update_vacation", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteVacation(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_vacation", CommandType.StoredProcedure, parameters);
        }
    }
}