using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IUserRepository
    {
        Task<UserIdentity> Authenticate(Credentials credentials);
    }
}
