using System.Text.Json;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Buslogix.Utilities;

namespace Buslogix.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {

        public async Task<UserIdentity?> Authenticate(Credentials credentials)
        {
            return await userRepository.Authenticate(credentials);
        }

        public async Task<User?> GetUser(int companyId, int id)
        {
            return await userRepository.GetUser(companyId, id);
        }

        public async Task<int> InsertUser(int companyId, User user)
        {
            user.Password = RandomGenerator.GenerateRandomString(10);
            return await userRepository.InsertUser(companyId, user);
        }

        public async Task<bool> UpdatePassword(int companyId, int id, string password)
        {
            int affected = await userRepository.UpdatePassword(companyId, id, password);
            return affected > 0;
        }

        public async Task<bool> UpdateOwnUser(int companyId, int id, User user)
        {
            int affected = await userRepository.UpdateOwnUser(companyId, id, user);
            return affected > 0;
        }

        public async Task<bool> UpdateUser(int companyId, int id, User user)
        {
            int affected = await userRepository.UpdateUser(companyId, id, user);
            return affected > 0;
        }

        public async Task<bool> DeleteUser(int companyId, int id)
        {
            int affected = await userRepository.DeleteUser(companyId, id);
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
            return await userRepository.GetUsers(companyId, roleId, isActive, identityDocument, name, lastName);
        }

        public async Task ResetPassword(Credentials credentials)
        {
            credentials.Password = RandomGenerator.GenerateRandomString(10);
            NotificationData? notificationData = await userRepository.ResetPassword(credentials);
            if (notificationData != null && notificationData.CompanyId > 0)
            {
                // TODO: Send email with new password
            }
        }

        public async Task<List<CriticalProcessUser>> GetCriticalProcessUsers(int companyId)
        {
            return await userRepository.GetCriticalProcessUsers(companyId);
        }

        public async Task<bool> UpdateCriticalProcessUsers(int companyId, List<CriticalProcessUser> users)
        {
            string usersJson = JsonSerializer.Serialize(users);
            int affected = await userRepository.UpdateCriticalProcessUsers(companyId, usersJson);
            return affected > 0;
        }

        public async Task<bool> IsCriticalProcessUser(int companyId, int id)
        {
            return await userRepository.IsCriticalProcessUser(companyId, id);
        }
    }
}
