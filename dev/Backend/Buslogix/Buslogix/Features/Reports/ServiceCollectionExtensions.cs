namespace Buslogix.Features.Reports
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReports(this IServiceCollection services)
        {
            services.AddScoped<IReportRepository, ReportRepository>();

            return services;
        }
    }
}
