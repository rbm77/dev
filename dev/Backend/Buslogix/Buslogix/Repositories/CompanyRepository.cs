using System.Data;
using System.Xml.Linq;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class CompanyRepository(IDataAccess dataAccess) : ICompanyRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<Company?> GetCompany()
        {
            List<Company> rows = await _dataAccess.ExecuteReader("get_company", CommandType.StoredProcedure,
                static reader => new Company
                {
                    Id = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1),
                    PhoneNumber = reader.GetStringOrDefault(2),
                    Email = reader.GetStringOrDefault(3),
                    IsActive = reader.GetBooleanOrDefault(4)
                }, null);

            return rows != null && rows.Count > 0 ? rows[0] : null;
        }

        public async Task<int> UpdateCompany(Company company)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_id"] = company.Id,
                ["p_name"] = company.Name,
                ["p_phone_number"] = company.PhoneNumber,
                ["p_email"] = company.Email,
                ["p_is_active"] = company.IsActive
            };
            return await _dataAccess.ExecuteNonQuery("update_company", CommandType.StoredProcedure, parameters);
        }
    }
}
