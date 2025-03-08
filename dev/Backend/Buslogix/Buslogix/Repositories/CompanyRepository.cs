using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Repositories
{
    public class CompanyRepository(IDataAccess dataAccess) : ICompanyRepository
    {
        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<List<Company>> GetCompanies()
        {
            string commandText = "GetCompanies";
            Dictionary<string, object> parameters = new()
            {
                { "@param1", "" },
                { "@param2", "" }
            };

            return await _dataAccess.ExecuteReader(commandText, CommandType.StoredProcedure,
                static reader => new Company
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    PhoneNumber = reader.GetString(2),
                    Email = reader.GetString(3),
                    IsActive = reader.GetBoolean(4)
                }, parameters);
        }
    }
}
