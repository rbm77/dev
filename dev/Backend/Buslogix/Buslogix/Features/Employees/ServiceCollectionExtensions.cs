namespace Buslogix.Features.Employees
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Employees feature: the employee repository.</summary>
        public static IServiceCollection AddEmployees(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
