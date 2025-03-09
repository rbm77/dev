using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface IUserService
    {
        Task<UserIdentity> Authenticate(Credentials credentials);
    }
}
