namespace Buslogix.Features.WhatsAppAccounts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWhatsAppAccounts(this IServiceCollection services)
        {
            services.AddScoped<IWhatsAppAccountRepository, WhatsAppAccountRepository>();

            return services;
        }
    }
}
