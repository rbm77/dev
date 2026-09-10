namespace Buslogix.Features.CustomTransports
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomTransports(this IServiceCollection services)
        {
            services.AddScoped<ICustomTransportRepository, CustomTransportRepository>();

            return services;
        }
    }
}
