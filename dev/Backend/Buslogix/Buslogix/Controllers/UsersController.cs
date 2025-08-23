using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(IUserService userService, ITokenHandler tokenHandler) : ControllerBase

    {
        private readonly IUserService _userService = userService;
        private readonly ITokenHandler _tokenHandler = tokenHandler;

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Credentials credentials)
        {
            UserIdentity? userIdentity = await _userService.Authenticate(credentials);
            if (userIdentity != null && userIdentity.IsAuthenticated)
            {
                Token? token = _tokenHandler.GenerateJwtToken(userIdentity);
                if (token != null)
                {
                    return Ok(token);
                }
            }
            return Unauthorized(new Error { Message = "Invalid credentials." });
        }
    }
}
