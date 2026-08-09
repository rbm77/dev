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
        Task<PagedResult<Role>> GetRoles(int companyId, string? description = null, int page = 1, int pageSize = 20);
        Task<int> UpdatePermissions(int companyId, int roleId, string permissionsJson);
        Task<PagedResult<RolePermission>> GetPermissions(int companyId, int roleId, int page = 1, int pageSize = 20);
    }
}
