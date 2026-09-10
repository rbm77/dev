namespace Buslogix.Features.Salaries
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSalaries(this IServiceCollection services)
        {
            services.AddScoped<ISalaryRepository, SalaryRepository>();

            return services;
        }
    }
}
