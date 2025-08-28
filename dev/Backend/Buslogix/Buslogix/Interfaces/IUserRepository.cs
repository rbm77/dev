using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IUserRepository
    {
        Task<UserIdentity?> Authenticate(Credentials credentials);
        Task<User?> GetUser(int companyId, int id);
        Task<int> InsertUser(int companyId, User user);
        Task<int> UpdatePassword(int companyId, int id, string password);
        Task<int> UpdateOwnUser(int companyId, int id, User user);
        Task<int> UpdateUser(int companyId, int id, User user);
        Task<int> DeleteUser(int companyId, int id);
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
