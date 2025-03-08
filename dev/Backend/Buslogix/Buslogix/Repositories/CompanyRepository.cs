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
            return await _dataAccess.ExecuteReader("GetCompanies", CommandType.StoredProcedure,
                static reader => new Company
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    PhoneNumber = reader.GetString(2),
                    Email = reader.GetString(3),
                    IsActive = reader.GetBoolean(4)
                }, null);
        }
    }
}
