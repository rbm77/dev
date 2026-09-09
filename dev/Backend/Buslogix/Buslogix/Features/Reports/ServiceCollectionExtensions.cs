namespace Buslogix.Features.Reports
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Reports feature: the report repository.</summary>
        public static IServiceCollection AddReports(this IServiceCollection services)
        {
            services.AddScoped<IReportRepository, ReportRepository>();

            return services;
        }
    }
}
