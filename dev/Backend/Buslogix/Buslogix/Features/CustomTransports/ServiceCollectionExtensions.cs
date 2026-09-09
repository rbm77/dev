namespace Buslogix.Features.CustomTransports
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the CustomTransports feature: the custom transport repository.</summary>
        public static IServiceCollection AddCustomTransports(this IServiceCollection services)
        {
            services.AddScoped<ICustomTransportRepository, CustomTransportRepository>();

            return services;
        }
    }
}
