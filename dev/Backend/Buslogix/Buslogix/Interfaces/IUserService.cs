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
        Task<List<User>> GetUsers(
            int companyId,
            int? roleId = null,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null
        );
    }
}
