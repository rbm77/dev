using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserIdentity?> Authenticate(Credentials credentials)
        {
            return await _userRepository.Authenticate(credentials);
        }

        public async Task<User?> GetUser(int companyId, int id)
        {
            return await _userRepository.GetUser(companyId, id);
        }

        public async Task<int> InsertUser(int companyId, User user)
        {
            return await _userRepository.InsertUser(companyId, user);
        }

        public async Task<bool> UpdatePassword(int companyId, int id, string password)
        {
            int affected = await _userRepository.UpdatePassword(companyId, id, password);
            return affected > 0;
        }

        public async Task<bool> UpdateOwnUser(int companyId, int id, User user)
        {
            int affected = await _userRepository.UpdateOwnUser(companyId, id, user);
            return affected > 0;
        }

        public async Task<bool> UpdateUser(int companyId, int id, User user)
        {
            int affected = await _userRepository.UpdateUser(companyId, id, user);
            return affected > 0;
        }

        public async Task<bool> DeleteUser(int companyId, int id)
        {
            int affected = await _userRepository.DeleteUser(companyId, id);
            return affected > 0;
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
            return await _userRepository.GetUsers(companyId, roleId, isActive, identityDocument, name, lastName);
        }
    }
}
