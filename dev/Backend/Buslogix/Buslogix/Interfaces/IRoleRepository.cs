using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRole(int companyId, int id);
        Task<int> InsertRole(int companyId, Role role);
        Task<int> UpdateRole(int companyId, int id, Role role);
        Task<int> DeleteRole(int companyId, int id);
        Task<List<Role>> GetRoles(int companyId, string? description = null);
    }
}
