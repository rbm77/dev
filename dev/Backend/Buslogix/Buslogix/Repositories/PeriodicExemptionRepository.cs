using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class PeriodicExemptionRepository(IDataAccess dataAccess) : IPeriodicExemptionRepository
    {

        public async Task<PeriodicExemption?> GetPeriodicExemption(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<PeriodicExemption> rows = await dataAccess.ExecuteReader("get_periodic_exemption", CommandType.StoredProcedure,
                static reader => new PeriodicExemption
                {
                    Id = reader.GetInt32OrDefault(0),
                    StudentId = reader.GetInt32OrDefault(1),
                    Description = reader.GetStringOrDefault(2),
                    StartDate = reader.GetDateTimeOrDefault(3),
                    EndDate = reader.GetDateTimeOrDefault(4),
                    Percentage = reader.GetDecimalOrDefault(5)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<PeriodicExemption>> GetPeriodicExemptions(
            int companyId,
            int? studentId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = studentId
            };

            List<PeriodicExemption> rows = await dataAccess.ExecuteReader("get_periodic_exemptions", CommandType.StoredProcedure,
                static reader => new PeriodicExemption
                {
                    Id = reader.GetInt32OrDefault(0),
                    StudentId = reader.GetInt32OrDefault(1),
                    Percentage = reader.GetDecimalOrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertPeriodicExemption(int companyId, PeriodicExemption exemption)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = exemption.StudentId,
                ["p_description"] = exemption.Description,
                ["p_start_date"] = exemption.StartDate,
                ["p_end_date"] = exemption.EndDate,
                ["p_percentage"] = exemption.Percentage
            };

            object? result = await dataAccess.ExecuteScalar("insert_periodic_exemption", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdatePeriodicExemption(int companyId, int id, PeriodicExemption exemption)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_student_id"] = exemption.StudentId,
                ["p_description"] = exemption.Description,
                ["p_start_date"] = exemption.StartDate,
                ["p_end_date"] = exemption.EndDate,
                ["p_percentage"] = exemption.Percentage
            };

            return await dataAccess.ExecuteNonQuery("update_periodic_exemption", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeletePeriodicExemption(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_periodic_exemption", CommandType.StoredProcedure, parameters);
        }
    }
}