using Buslogix.Models.DTO;

namespace Buslogix.Interfaces
{
    public interface ITokenHandler
    {

        Token? GenerateJwtToken(UserIdentity userIdentity);
    }
}
