
namespace Buslogix.Features.Users
{
    public interface ITokenHandler
    {

        Token? GenerateToken(UserIdentity userIdentity);
    }
}
