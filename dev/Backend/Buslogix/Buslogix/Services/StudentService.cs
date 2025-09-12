using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class StudentService(IStudentRepository studentRepository) : IStudentService
    {

        private readonly IStudentRepository _studentRepository = studentRepository;

        public async Task<Student?> GetStudent(int companyId, int id)
        {
            return await _studentRepository.GetStudent(companyId, id);
        }

        public async Task<List<Student>> GetStudents(
            int companyId,
            bool? isActive = null,
            string? identityDocument = null,
            string? name = null,
            string? lastName = null,
            int? routeId = null,
            int? gradeId = null
        )
        {
            return await _studentRepository.GetStudents(companyId, isActive, identityDocument, name, lastName, routeId, gradeId);
        }

        public async Task<int> InsertStudent(int companyId, Student student)
        {
            return await _studentRepository.InsertStudent(companyId, student);
        }

        public async Task<bool> UpdateStudent(int companyId, int id, Student student)
        {
            int affected = await _studentRepository.UpdateStudent(companyId, id, student);
            return affected > 0;
        }

        public async Task<bool> DeleteStudent(int companyId, int id)
        {
            int affected = await _studentRepository.DeleteStudent(companyId, id);
            return affected > 0;
        }
    }
}