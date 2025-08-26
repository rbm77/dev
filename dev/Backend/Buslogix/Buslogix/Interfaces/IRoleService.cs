using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> GetRole(int companyId, int id);
        Task<int> InsertRole(int companyId, Role role);
        Task<bool> UpdateRole(int companyId, int id, Role role);
        Task<bool> DeleteRole(int companyId, int id);
        Task<List<Role>> GetRoles(int companyId, string? description = null);
    }
}
