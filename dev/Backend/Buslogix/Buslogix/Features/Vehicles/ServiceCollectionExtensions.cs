namespace Buslogix.Features.Vehicles
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVehicles(this IServiceCollection services)
        {
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            return services;
        }
    }
}
