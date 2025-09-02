using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRole(int companyId, int id);
        Task<int> InsertRole(int companyId, Role role);
        Task<int> UpdateRole(int companyId, int id, Role role);
        Task<int> DeleteRole(int companyId, int id);
        Task<List<Role>> GetRoles(int companyId, string? description = null);
        Task<int> UpdatePermissions(int companyId, int roleId, string permissionsJson);
        Task<List<RolePermission>> GetPermissions(int companyId, int roleId);
    }
}
