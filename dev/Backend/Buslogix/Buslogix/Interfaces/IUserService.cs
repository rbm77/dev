using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IUserService
    {
        Task<UserIdentity?> Authenticate(Credentials credentials);
        Task ResetPassword(Credentials credentials);
        Task<User?> GetUser(int companyId, int id);
        Task<int> InsertUser(int companyId, User user);
        Task<bool> UpdatePassword(int companyId, int id, string password);
        Task<bool> UpdateOwnUser(int companyId, int id, User user);
        Task<bool> UpdateUser(int companyId, int id, User user);
        Task<bool> DeleteUser(int companyId, int id);
        Task<PagedResult<User>> GetUsers(
            int companyId,
            int? roleId = null,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null,
            int page = 1,
            int pageSize = 20
        );
        Task<PagedResult<CriticalProcessUser>> GetCriticalProcessUsers(int companyId, int page = 1, int pageSize = 20);
        Task<bool> UpdateCriticalProcessUsers(int companyId, List<CriticalProcessUser> users);
        Task<bool> IsCriticalProcessUser(int companyId, int id);
    }
}
