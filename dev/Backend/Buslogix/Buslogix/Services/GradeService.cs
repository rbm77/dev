using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class GradeService(IGradeRepository gradeRepository) : IGradeService
    {

        private readonly IGradeRepository _gradeRepository = gradeRepository;

        public async Task<Grade?> GetGrade(int companyId, int id)
        {
            return await _gradeRepository.GetGrade(companyId, id);
        }

        public async Task<List<Grade>> GetGrades(int companyId, string? description = null)
        {
            return await _gradeRepository.GetGrades(companyId, description);
        }

        public async Task<int> InsertGrade(int companyId, Grade grade)
        {
            return await _gradeRepository.InsertGrade(companyId, grade);
        }

        public async Task<bool> UpdateGrade(int companyId, int id, Grade grade)
        {
            int affected = await _gradeRepository.UpdateGrade(companyId, id, grade);
            return affected > 0;
        }

        public async Task<bool> DeleteGrade(int companyId, int id)
        {
            int affected = await _gradeRepository.DeleteGrade(companyId, id);
            return affected > 0;
        }
    }
}