using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface ITokenHandler
    {

        Token? GenerateToken(UserIdentity userIdentity);
    }
}
