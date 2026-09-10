namespace Buslogix.Features.EmailSenders
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailSenders(this IServiceCollection services)
        {
            services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();

            return services;
        }
    }
}
