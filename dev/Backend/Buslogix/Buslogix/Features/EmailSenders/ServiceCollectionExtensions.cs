namespace Buslogix.Features.EmailSenders
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the EmailSenders feature: the email sender repository.</summary>
        public static IServiceCollection AddEmailSenders(this IServiceCollection services)
        {
            services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();

            return services;
        }
    }
}
