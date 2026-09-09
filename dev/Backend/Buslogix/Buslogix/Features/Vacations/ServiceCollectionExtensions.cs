namespace Buslogix.Features.Vacations
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Vacations feature: the vacation repository.</summary>
        public static IServiceCollection AddVacations(this IServiceCollection services)
        {
            services.AddScoped<IVacationRepository, VacationRepository>();

            return services;
        }
    }
}
