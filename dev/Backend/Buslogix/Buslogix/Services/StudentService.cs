using System.Text.Json;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;

namespace Buslogix.Services
{
    public class StudentService(IStudentRepository studentRepository, IQrCodeService qrCodeService) : IStudentService
    {

        public async Task<Student?> GetStudent(int companyId, int id)
        {
            return await studentRepository.GetStudent(companyId, id);
        }

        public async Task<List<QrCodeResponseItem>> GenerateStudentQrCodes(int companyId, List<int> studentIds, int size)
        {
            string idsJson = JsonSerializer.Serialize(studentIds);
            List<Student> students = await studentRepository.GetStudentsByIds(companyId, idsJson);

            List<QrCodeRequestItem> items = [];
            foreach (Student student in students)
            {
                items.Add(new QrCodeRequestItem
                {
                    Value = student.Id.ToString(),
                    Description = $"{student.Name} {student.LastName}".Trim()
                });
            }

            return qrCodeService.GenerateQrCodes(items, size);
        }

        public async Task<PagedResult<Student>> GetStudents(
            int companyId,
            bool? isActive,
            string? identityDocument,
            string? name,
            string? lastName,
            int? routeId,
            int? gradeId,
            int page,
            int pageSize
        )
        {
            return await studentRepository.GetStudents(companyId, isActive, identityDocument, name, lastName, routeId, gradeId, page, pageSize);
        }

        public async Task<int> InsertStudent(int companyId, Student student)
        {
            return await studentRepository.InsertStudent(companyId, student);
        }

        public async Task<bool> UpdateStudent(int companyId, int id, Student student)
        {
            int affected = await studentRepository.UpdateStudent(companyId, id, student);
            return affected > 0;
        }

        public async Task<bool> DeleteStudent(int companyId, int id)
        {
            int affected = await studentRepository.DeleteStudent(companyId, id);
            return affected > 0;
        }
    }
}