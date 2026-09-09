namespace Buslogix.Features.Routes
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Routes feature: the route repository.</summary>
        public static IServiceCollection AddRoutes(this IServiceCollection services)
        {
            services.AddScoped<IRouteRepository, RouteRepository>();

            return services;
        }
    }
}
