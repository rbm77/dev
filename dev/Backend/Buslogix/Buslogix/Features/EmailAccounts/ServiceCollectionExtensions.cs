namespace Buslogix.Features.EmailAccounts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailAccounts(this IServiceCollection services)
        {
            services.AddScoped<IEmailAccountRepository, EmailAccountRepository>();

            return services;
        }
    }
}
