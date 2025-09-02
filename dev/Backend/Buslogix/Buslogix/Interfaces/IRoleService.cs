using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> GetRole(int companyId, int id);
        Task<int> InsertRole(int companyId, Role role);
        Task<bool> UpdateRole(int companyId, int id, Role role);
        Task<bool> DeleteRole(int companyId, int id);
        Task<List<Role>> GetRoles(int companyId, string? description = null);
        Task<bool> UpdatePermissions(int companyId, int roleId, List<RolePermission> permissions);
        Task<List<RolePermission>> GetPermissions(int companyId, int roleId);
    }
}
