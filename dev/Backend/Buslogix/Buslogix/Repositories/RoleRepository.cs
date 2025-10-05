using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class RoleRepository(IDataAccess dataAccess) : IRoleRepository
    {

        public async Task<Role?> GetRole(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };
            List<Role> rows = await dataAccess.ExecuteReader("get_role", CommandType.StoredProcedure,
                static reader => new Role
                {
                    Id = reader.GetInt32OrDefault(0),
                    Description = reader.GetStringOrDefault(1)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<int> InsertRole(int companyId, Role role)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_description"] = role.Description
            };
            object? result = await dataAccess.ExecuteScalar("insert_role", CommandType.StoredProcedure, parameters);
            return result != null ? ((int)result) : 0;
        }

        public async Task<int> UpdateRole(int companyId, int id, Role role)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id,
                ["p_description"] = role.Description
            };
            return await dataAccess.ExecuteNonQuery("update_role", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteRole(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };
            return await dataAccess.ExecuteNonQuery("delete_role", CommandType.StoredProcedure, parameters);
        }

        public async Task<List<Role>> GetRoles(int companyId, string? description = null)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_description"] = description
            };
            List<Role> rows = await dataAccess.ExecuteReader("get_roles", CommandType.StoredProcedure,
                static reader => new Role
                {
                    Id = reader.GetInt32OrDefault(0),
                    Description = reader.GetStringOrDefault(1)
                }, parameters);

            return rows;
        }

        public async Task<int> UpdatePermissions(int companyId, int roleId, string permissionsJson)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_role_id"] = roleId,
                ["p_permissions_json"] = permissionsJson
            };

            object? result = await dataAccess.ExecuteScalar("update_permissions", CommandType.StoredProcedure, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<List<RolePermission>> GetPermissions(int companyId, int roleId)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_role_id"] = roleId
            };

            return await dataAccess.ExecuteReader(
                "get_permissions",
                CommandType.StoredProcedure,
                static reader => new RolePermission
                {
                    ResourceId = reader.GetInt32OrDefault(0),
                    Description = reader.GetStringOrDefault(1),
                    IsAssigned = reader.GetBooleanOrDefault(2),
                    IsEditable = reader.GetBooleanOrDefault(3)
                },
                parameters
            );
        }
    }
}
