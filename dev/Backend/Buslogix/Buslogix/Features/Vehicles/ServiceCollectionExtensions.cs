namespace Buslogix.Features.Vehicles
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Vehicles feature: the vehicle repository.</summary>
        public static IServiceCollection AddVehicles(this IServiceCollection services)
        {
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            return services;
        }
    }
}
