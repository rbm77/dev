using System.Security.Cryptography;
using System.Text;
using Buslogix.Features.WhatsAppBot.Abstractions;
using Buslogix.Features.WhatsAppBot.Configuration;
using Microsoft.Extensions.Options;

namespace Buslogix.Features.WhatsAppBot.Webhook
{
    public class WhatsAppSignatureValidator(IOptionsMonitor<WhatsAppOptions> optionsMonitor) : IWhatsAppSignatureValidator
    {
        private const string SignaturePrefix = "sha256=";

        public bool IsValid(byte[] payload, string? signatureHeader)
        {
            if (string.IsNullOrWhiteSpace(signatureHeader) || !signatureHeader.StartsWith(SignaturePrefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string appSecret = optionsMonitor.CurrentValue.AppSecret;
            if (string.IsNullOrWhiteSpace(appSecret))
            {
                return false;
            }

            string presentedHex = signatureHeader[SignaturePrefix.Length..];

            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(appSecret));
            byte[] computedHash = hmac.ComputeHash(payload);
            string computedHex = Convert.ToHexStringLower(computedHash);

            byte[] a = Encoding.UTF8.GetBytes(presentedHex);
            byte[] b = Encoding.UTF8.GetBytes(computedHex);
            return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
