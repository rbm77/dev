using Buslogix.EmailIngestion.Abstractions;
using Buslogix.EmailIngestion.Imap;
using Buslogix.EmailIngestion.Parsing;

namespace Buslogix.EmailIngestion
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the whole EmailIngestion feature: the IMAP adapter, the
        /// HTML/plain-text body extractor, and the orchestrator. Assumes
        /// IEmailAccountRepository/IEmailSenderRepository (administrative CRUD
        /// layer) are already registered.
        /// </summary>
        public static IServiceCollection AddEmailIngestion(this IServiceCollection services)
        {
            services.AddScoped<IEmailBodyTextExtractor, EmailBodyTextExtractor>();
            services.AddScoped<IEmailClient, MailKitEmailClient>();
            services.AddScoped<IEmailIngestionService, EmailIngestionService>();

            return services;
        }
    }
}
