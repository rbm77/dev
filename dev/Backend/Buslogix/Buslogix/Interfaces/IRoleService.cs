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
        Task<PagedResult<Role>> GetRoles(int companyId, string? description = null, int page = 1, int pageSize = 20);
        Task<bool> UpdatePermissions(int companyId, int roleId, List<RolePermission> permissions);
        Task<PagedResult<RolePermission>> GetPermissions(int companyId, int roleId, int page = 1, int pageSize = 20);
    }
}
