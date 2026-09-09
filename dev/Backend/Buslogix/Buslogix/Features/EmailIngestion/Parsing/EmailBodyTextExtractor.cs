using Buslogix.Features.EmailIngestion.Abstractions;
using HtmlAgilityPack;
using MimeKit;

namespace Buslogix.Features.EmailIngestion.Parsing
{
    public class EmailBodyTextExtractor : IEmailBodyTextExtractor
    {
        public string ExtractPlainText(MimeMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message.HtmlBody))
            {
                HtmlDocument document = new();
                document.LoadHtml(message.HtmlBody);
                return document.DocumentNode.InnerText;
            }

            return message.TextBody ?? string.Empty;
        }
    }
}
