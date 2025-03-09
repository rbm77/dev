using Buslogix.Interfaces;
using Buslogix.Models.DTO;

namespace Buslogix.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<UserIdentity> Authenticate(Credentials credentials)
        {
            return new UserIdentity()
            {
                IsAuthenticated = true
            };
        }
    }
}
