using Buslogix.Features.EmailIngestion.Abstractions;
using Buslogix.Features.EmailIngestion.Imap;
using Buslogix.Features.EmailIngestion.Parsing;

namespace Buslogix.Features.EmailIngestion
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the whole EmailIngestion feature: the IMAP adapter, the
        /// HTML/plain-text body extractor, and the orchestrator. Assumes
        /// IEmailAccountRepository/IEmailSenderRepository (administrative CRUD
        /// layer) are already registered. EmailIngestionService also depends
        /// on IMessageIngestionQueue (Features/MessageExtraction) -
        /// AddMessageExtraction() must also be called, in either order.
        /// </summary>
        public static IServiceCollection AddEmailIngestion(this IServiceCollection services)
        {
            services.AddScoped<IEmailBodyTextExtractor, EmailBodyTextExtractor>();
            services.AddScoped<IEmailClient, MailKitEmailClient>();
            services.AddScoped<IEmailIngestionService, EmailIngestionService>();

            // Trigger for the "poll" endpoint - its TriggerWorker is wired up
            // in Program.cs, since it needs the ProcessAllAccountsAsync
            // delegate composed there.
            services.AddSingleton<EmailPollQueue>();

            return services;
        }
    }
}
