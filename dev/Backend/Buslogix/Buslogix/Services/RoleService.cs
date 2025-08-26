using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<Role?> GetRole(int companyId, int id)
        {
            return await _roleRepository.GetRole(companyId, id);
        }

        public async Task<int> InsertRole(int companyId, Role role)
        {
            return await _roleRepository.InsertRole(companyId, role);
        }

        public async Task<bool> UpdateRole(int companyId, int id, Role role)
        {
            return (await _roleRepository.UpdateRole(companyId, id, role)) > 0;
        }

        public async Task<bool> DeleteRole(int companyId, int id)
        {
            return (await _roleRepository.DeleteRole(companyId, id)) > 0;
        }

        public async Task<List<Role>> GetRoles(int companyId, string? description = null)
        {
            return await _roleRepository.GetRoles(companyId, description);
        }
    }
}
