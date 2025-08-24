using Buslogix.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Buslogix.Interfaces;
using Buslogix.Utilities;

namespace Buslogix.Handlers
{
    public class TokenHandler(IConfiguration configuration) : ITokenHandler
    {

        private readonly IConfiguration _configuration = configuration;

        public Token? GenerateJwtToken(UserIdentity userIdentity)
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
                    new Claim("permissions", string.Join(",", permissionCodes)),
                    new Claim("cid", userIdentity.CompanyId.ToString())
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
