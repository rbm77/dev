using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IStudentService
    {

        Task<Student?> GetStudent(int companyId, int id);

        Task<PagedResult<Student>> GetStudents(
            int companyId,
            bool? isActive,
            string? identityDocument,
            string? name,
            string? lastName,
            int? routeId,
            int? gradeId,
            int page,
            int pageSize
        );

        Task<int> InsertStudent(int companyId, Student student);

        Task<bool> UpdateStudent(int companyId, int id, Student student);

        Task<bool> DeleteStudent(int companyId, int id);
    }
}
