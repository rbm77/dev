namespace Buslogix.Features.Contacts
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Contacts feature: the contact repository.</summary>
        public static IServiceCollection AddContacts(this IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();

            return services;
        }
    }
}
