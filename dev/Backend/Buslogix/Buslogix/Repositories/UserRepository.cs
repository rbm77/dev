using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models.DTO;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class UserRepository(IDataAccess dataAccess) : IUserRepository
    {

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<UserIdentity?> Authenticate(Credentials credentials)
        {
            List<UserIdentity> rows = await _dataAccess.ExecuteReader(
                           "authenticate_user",
                           CommandType.StoredProcedure,
                           static reader => {
                               string permissions = reader.GetStringOrDefault(3, "");
                               return new UserIdentity
                               {
                                   CompanyId = reader.GetInt32OrDefault(0),
                                   Id = reader.GetInt32OrDefault(1),
                                   Username = reader.GetStringOrDefault(2),
                                   Permissions = string.IsNullOrEmpty(permissions)
                                       ? []
                                       : [.. permissions.Split(',')],
                                   IsAuthenticated = reader.GetBooleanOrDefault(4)
                               };
                           },
                           new Dictionary<string, object?>
                           {
                               ["p_username"] = credentials.Username,
                               ["p_password"] = credentials.Password
                           }
                       );

            return rows != null && rows.Count > 0 ? rows[0] : null;
        }
    }
}
