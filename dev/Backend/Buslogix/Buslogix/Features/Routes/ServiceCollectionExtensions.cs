namespace Buslogix.Features.Routes
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRoutes(this IServiceCollection services)
        {
            services.AddScoped<IRouteRepository, RouteRepository>();

            return services;
        }
    }
}
