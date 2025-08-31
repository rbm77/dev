using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
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

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<User?> GetUser(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<User> rows = await _dataAccess.ExecuteReader("get_user", CommandType.StoredProcedure,
                static reader => new User
                {
                    Id = reader.GetInt32OrDefault(0),
                    Username = reader.GetStringOrDefault(1),
                    RoleId = reader.GetInt32OrDefault(2),
                    IsActive = reader.GetBooleanOrDefault(3),

                    IdentityDocument = reader.GetStringOrDefault(4),
                    Name = reader.GetStringOrDefault(5),
                    LastName = reader.GetStringOrDefault(6),
                    Address = reader.GetStringOrDefault(7),
                    PhoneNumber = reader.GetStringOrDefault(8),
                    Email = reader.GetStringOrDefault(9)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<int> InsertUser(int companyId, User user)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_username"] = user.Username,
                ["p_password"] = user.Password,
                ["p_role_id"] = user.RoleId,
                ["p_is_active"] = user.IsActive,

                ["p_identity_document"] = user.IdentityDocument,
                ["p_name"] = user.Name,
                ["p_lastname"] = user.LastName,
                ["p_address"] = user.Address,
                ["p_phone_number"] = user.PhoneNumber,
                ["p_email"] = user.Email
            };

            object? result = await _dataAccess.ExecuteScalar("insert_user", CommandType.StoredProcedure, parameters);
            return result != null ? ((int)result) : 0;
        }

        public async Task<int> UpdatePassword(int companyId, int id, string password)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_password"] = password
            };

            return await _dataAccess.ExecuteNonQuery("update_user_password", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> UpdateOwnUser(int companyId, int id, User user)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_username"] = user.Username,
                ["p_identity_document"] = user.IdentityDocument,
                ["p_name"] = user.Name,
                ["p_lastname"] = user.LastName,
                ["p_address"] = user.Address,
                ["p_phone_number"] = user.PhoneNumber,
                ["p_email"] = user.Email
            };

            return await _dataAccess.ExecuteNonQuery("update_own_user", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> UpdateUser(int companyId, int id, User user)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_username"] = user.Username,
                ["p_role_id"] = user.RoleId,
                ["p_is_active"] = user.IsActive,

                ["p_identity_document"] = user.IdentityDocument,
                ["p_name"] = user.Name,
                ["p_lastname"] = user.LastName,
                ["p_address"] = user.Address,
                ["p_phone_number"] = user.PhoneNumber,
                ["p_email"] = user.Email
            };

            return await _dataAccess.ExecuteNonQuery("update_user", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteUser(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            return await _dataAccess.ExecuteNonQuery("delete_user", CommandType.StoredProcedure, parameters);
        }

        public async Task<List<User>> GetUsers(
            int companyId,
            int? roleId = null,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null
        )
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_role_id"] = roleId,
                ["p_is_active"] = isActive,
                ["p_identity_document"] = identityDocument,
                ["p_name"] = name,
                ["p_lastname"] = lastName
            };

            List<User> rows = await _dataAccess.ExecuteReader("get_users", CommandType.StoredProcedure,
                static reader => new User
                {
                    Id = reader.GetInt32OrDefault(0),
                    Username = reader.GetStringOrDefault(1),
                    RoleId = reader.GetInt32OrDefault(2),
                    IsActive = reader.GetBooleanOrDefault(3),

                    IdentityDocument = reader.GetStringOrDefault(4),
                    Name = reader.GetStringOrDefault(5),
                    LastName = reader.GetStringOrDefault(6),
                    Address = reader.GetStringOrDefault(7),
                    PhoneNumber = reader.GetStringOrDefault(8),
                    Email = reader.GetStringOrDefault(9)
                }, parameters);

            return rows;
        }

        public async Task<NotificationData?> ResetPassword(Credentials credentials)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_username"] = credentials.Username,
                ["p_password"] = credentials.Password
            };

            List<NotificationData> rows = await _dataAccess.ExecuteReader("reset_password", CommandType.StoredProcedure,
                static reader => new NotificationData
                {
                    CompanyId = reader.GetInt32OrDefault(0),
                    Name = reader.GetStringOrDefault(1),
                    LastName = reader.GetStringOrDefault(2),
                    Email = reader.GetStringOrDefault(3),
                    PhoneNumber = reader.GetStringOrDefault(4)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }
    }
}



