using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Buslogix.Interfaces;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using static Buslogix.Utilities.Enums;

namespace Buslogix.Handlers
{
    public class ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration,
        ILogHandler logHandler)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ServiceAuth.ApiKeyHeaderName, out StringValues headerValues))
            {
                return AuthenticateResult.NoResult();
            }

            string? presentedKey = headerValues.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(presentedKey))
            {
                return AuthenticateResult.NoResult();
            }

            string? matchedService = null;
            foreach (IConfigurationSection entry in configuration.GetSection("ServiceApiKeys").GetChildren())
            {
                if (!string.IsNullOrEmpty(entry.Value) && FixedTimeEquals(presentedKey, entry.Value))
                {
                    matchedService = entry.Key;
                    break;
                }
            }

            if (matchedService == null)
            {
                await logHandler.WriteLog("Invalid API key presented.", LogType.Warning);
                return AuthenticateResult.Fail("Invalid API key.");
            }

            Claim[] claims =
            [
                new Claim(ClaimTypes.Name, matchedService),
                new Claim(ServiceAuth.ServiceTokenClaimType, ServiceAuth.ServiceTokenClaimValue),
                new Claim(ServiceAuth.ServiceNameClaimType, matchedService)
            ];
            ClaimsIdentity identity = new(claims, Scheme.Name);
            AuthenticationTicket ticket = new(new ClaimsPrincipal(identity), Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        private static bool FixedTimeEquals(string presented, string configured)
        {
            byte[] a = Encoding.UTF8.GetBytes(presented);
            byte[] b = Encoding.UTF8.GetBytes(configured);
            return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
