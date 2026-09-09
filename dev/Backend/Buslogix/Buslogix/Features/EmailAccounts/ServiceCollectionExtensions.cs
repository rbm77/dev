namespace Buslogix.Features.EmailAccounts
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the EmailAccounts feature: the email account repository.</summary>
        public static IServiceCollection AddEmailAccounts(this IServiceCollection services)
        {
            services.AddScoped<IEmailAccountRepository, EmailAccountRepository>();

            return services;
        }
    }
}
