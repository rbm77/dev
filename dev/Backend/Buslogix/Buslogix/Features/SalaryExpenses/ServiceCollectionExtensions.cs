namespace Buslogix.Features.SalaryExpenses
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSalaryExpenses(this IServiceCollection services)
        {
            services.AddScoped<ISalaryExpenseRepository, SalaryExpenseRepository>();

            return services;
        }
    }
}
