namespace Buslogix.Features.Students
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Students feature: the student repository, the
        /// student service, and the QR code service it depends on (used to
        /// generate/scan student QR codes, not a cross-cutting concern -
        /// StudentService is its only consumer).
        /// </summary>
        public static IServiceCollection AddStudents(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IQrCodeService, QrCodeService>();

            return services;
        }
    }
}
