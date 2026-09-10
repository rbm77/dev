namespace Buslogix.Features.Vacations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVacations(this IServiceCollection services)
        {
            services.AddScoped<IVacationRepository, VacationRepository>();

            return services;
        }
    }
}
