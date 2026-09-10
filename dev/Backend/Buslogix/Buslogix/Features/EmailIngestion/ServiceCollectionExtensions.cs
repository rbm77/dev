using Buslogix.Features.EmailIngestion.Abstractions;
using Buslogix.Features.EmailIngestion.Imap;
using Buslogix.Features.EmailIngestion.Parsing;

namespace Buslogix.Features.EmailIngestion
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailIngestion(this IServiceCollection services)
        {
            services.AddScoped<IEmailBodyTextExtractor, EmailBodyTextExtractor>();
            services.AddScoped<IEmailClient, MailKitEmailClient>();
            services.AddScoped<IEmailIngestionService, EmailIngestionService>();

            services.AddSingleton<EmailPollQueue>();

            return services;
        }
    }
}
