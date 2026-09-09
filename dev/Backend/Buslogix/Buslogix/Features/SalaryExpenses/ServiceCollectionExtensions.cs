namespace Buslogix.Features.SalaryExpenses
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the SalaryExpenses feature: the salary expense repository.</summary>
        public static IServiceCollection AddSalaryExpenses(this IServiceCollection services)
        {
            services.AddScoped<ISalaryExpenseRepository, SalaryExpenseRepository>();

            return services;
        }
    }
}
