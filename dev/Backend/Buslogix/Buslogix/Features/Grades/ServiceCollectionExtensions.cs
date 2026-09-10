namespace Buslogix.Features.Grades
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrades(this IServiceCollection services)
        {
            services.AddScoped<IGradeRepository, GradeRepository>();

            return services;
        }
    }
}
