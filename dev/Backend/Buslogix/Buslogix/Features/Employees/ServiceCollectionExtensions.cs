namespace Buslogix.Features.Employees
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployees(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
