namespace Buslogix.Features.Contacts
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContacts(this IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();

            return services;
        }
    }
}
