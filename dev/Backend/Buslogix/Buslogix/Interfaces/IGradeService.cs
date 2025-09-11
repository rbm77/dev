using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IGradeService
    {

        Task<Grade?> GetGrade(int companyId, int id);

        Task<List<Grade>> GetGrades(int companyId, string? description = null);

        Task<int> InsertGrade(int companyId, Grade grade);

        Task<bool> UpdateGrade(int companyId, int id, Grade grade);

        Task<bool> DeleteGrade(int companyId, int id);
    }
}
