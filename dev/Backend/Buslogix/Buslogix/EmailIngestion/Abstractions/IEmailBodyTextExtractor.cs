using MimeKit;

namespace Buslogix.EmailIngestion.Abstractions
{
    public interface IEmailBodyTextExtractor
    {
        /// <summary>Prefers the HTML part (stripped to plain text); falls back to the plain text part.</summary>
        string ExtractPlainText(MimeMessage message);
    }
}
