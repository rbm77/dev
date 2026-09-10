namespace Buslogix.Features.Students
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStudents(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IQrCodeService, QrCodeService>();

            return services;
        }
    }
}
