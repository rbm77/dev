namespace Buslogix.Features.Salaries
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Salaries feature: the salary repository.</summary>
        public static IServiceCollection AddSalaries(this IServiceCollection services)
        {
            services.AddScoped<ISalaryRepository, SalaryRepository>();

            return services;
        }
    }
}
