using System.Text.Json;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;

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

        public async Task<List<RolePermission>> GetPermissions(int companyId, int roleId)
        {
            return await _roleRepository.GetPermissions(companyId, roleId);
        }

        public async Task<bool> UpdatePermissions(int companyId, int roleId, List<RolePermission> permissions)
        {
            string permissionsJson = JsonSerializer.Serialize(permissions);
            int affected = await _roleRepository.UpdatePermissions(companyId, roleId, permissionsJson);
            return affected > 0;
        }
    }
}
