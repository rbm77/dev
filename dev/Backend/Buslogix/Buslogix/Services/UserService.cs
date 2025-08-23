using Buslogix.Interfaces;
using Buslogix.Models.DTO;

namespace Buslogix.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public Task<UserIdentity?> Authenticate(Credentials credentials)
        {
            return _userRepository.Authenticate(credentials);
        }
    }
}
