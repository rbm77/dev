using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class GradeRepository(IDataAccess dataAccess) : IGradeRepository
    {

        public async Task<Grade?> GetGrade(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<Grade> rows = await dataAccess.ExecuteReader("get_grade", CommandType.StoredProcedure,
                static reader => new Grade
                {
                    Id = reader.GetInt32OrDefault(0),
                    Description = reader.GetStringOrDefault(1)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<Grade>> GetGrades(int companyId, string? description = null)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_description"] = description
            };

            List<Grade> rows = await dataAccess.ExecuteReader("get_grades", CommandType.StoredProcedure,
                static reader => new Grade
                {
                    Id = reader.GetInt32OrDefault(0),
                    Description = reader.GetStringOrDefault(1)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertGrade(int companyId, Grade grade)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_description"] = grade.Description
            };

            object? result = await dataAccess.ExecuteScalar("insert_grade", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateGrade(int companyId, int id, Grade grade)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_description"] = grade.Description
            };

            return await dataAccess.ExecuteNonQuery("update_grade", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteGrade(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_grade", CommandType.StoredProcedure, parameters);
        }
    }
}