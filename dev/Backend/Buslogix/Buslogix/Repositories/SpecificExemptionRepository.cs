using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class SpecificExemptionRepository(IDataAccess dataAccess) : ISpecificExemptionRepository
    {

        public async Task<SpecificExemption?> GetSpecificExemption(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<SpecificExemption> rows = await dataAccess.ExecuteReader("get_specific_exemption", CommandType.StoredProcedure,
                static reader => new SpecificExemption
                {
                    Id = reader.GetInt32OrDefault(0),
                    StudentId = reader.GetInt32OrDefault(1),
                    PaymentPeriodId = reader.GetInt32OrDefault(2),
                    Description = reader.GetStringOrDefault(3),
                    Percentage = reader.GetDecimalOrDefault(4)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<List<SpecificExemption>> GetSpecificExemptions(
            int companyId,
            int? studentId = null,
            int? paymentPeriodId = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = studentId,
                ["p_payment_period"] = paymentPeriodId
            };

            List<SpecificExemption> rows = await dataAccess.ExecuteReader("get_specific_exemptions", CommandType.StoredProcedure,
                static reader => new SpecificExemption
                {
                    Id = reader.GetInt32OrDefault(0),
                    StudentId = reader.GetInt32OrDefault(1),
                    PaymentPeriodId = reader.GetInt32OrDefault(2)
                }, parameters);

            return rows;
        }

        public async Task<int> InsertSpecificExemption(int companyId, SpecificExemption exemption)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = exemption.StudentId,
                ["p_payment_period"] = exemption.PaymentPeriodId,
                ["p_description"] = exemption.Description,
                ["p_percentage"] = exemption.Percentage
            };

            object? result = await dataAccess.ExecuteScalar("insert_specific_exemption", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateSpecificExemption(int companyId, int id, SpecificExemption exemption)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_student_id"] = exemption.StudentId,
                ["p_payment_period"] = exemption.PaymentPeriodId,
                ["p_description"] = exemption.Description,
                ["p_percentage"] = exemption.Percentage
            };

            return await dataAccess.ExecuteNonQuery("update_specific_exemption", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteSpecificExemption(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_specific_exemption", CommandType.StoredProcedure, parameters);
        }
    }
}
