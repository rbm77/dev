using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IStudentRepository
    {

        Task<Student?> GetStudent(int companyId, int id);

        Task<List<Student>> GetStudents(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null,
            int? routeId = null,
            int? gradeId = null
        );

        Task<int> InsertStudent(int companyId, Student student);

        Task<int> UpdateStudent(int companyId, int id, Student student);

        Task<int> DeleteStudent(int companyId, int id);
    }
}