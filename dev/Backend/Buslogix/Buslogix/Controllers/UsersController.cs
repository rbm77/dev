using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Buslogix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IConfiguration configuration, IUserService userService) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserService _userService = userService;

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Credentials credentials)
        {
            UserIdentity userIdentity = await _userService.Authenticate(credentials);
            if (userIdentity != null && userIdentity.IsAuthenticated)
            {
                Token? token = GenerateJwtToken(userIdentity);
                if (token != null)
                {
                    return Ok(token);
                }
            }
            return Unauthorized(new Error { Message = "Invalid credentials." });
        }

        private Token? GenerateJwtToken(UserIdentity userIdentity)
        {
            if (userIdentity?.Permissions != null && userIdentity.Permissions.Count > 0)
            {
                string[] permissionCodes = userIdentity.Permissions
                    .Select(static p => PermissionMap.PermissionToCode[p])
                    .ToArray();

                List<Claim> claims =
                [
                    new Claim(ClaimTypes.Name, userIdentity.Username ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, userIdentity.Id.ToString()),
                    new Claim("permissions", string.Join(",", permissionCodes))
                ];

                string secretKey = _configuration["JWT:SecretKey"] ?? "";
                SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
                SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                return new()
                {
                    AuthenticationToken = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            return null;
        }
    }
}
