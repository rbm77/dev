using MimeKit;

namespace Buslogix.Features.EmailIngestion.Abstractions
{
    public interface IEmailBodyTextExtractor
    {
        string ExtractPlainText(MimeMessage message);
    }
}
