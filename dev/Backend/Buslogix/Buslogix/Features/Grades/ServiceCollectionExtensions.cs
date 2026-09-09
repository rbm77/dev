namespace Buslogix.Features.Grades
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers the Grades feature: the grade repository.</summary>
        public static IServiceCollection AddGrades(this IServiceCollection services)
        {
            services.AddScoped<IGradeRepository, GradeRepository>();

            return services;
        }
    }
}
