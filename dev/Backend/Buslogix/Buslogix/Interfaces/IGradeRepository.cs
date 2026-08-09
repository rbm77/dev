using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IGradeRepository
    {

        Task<Grade?> GetGrade(int companyId, int id);

        Task<PagedResult<Grade>> GetGrades(int companyId, string? description = null, int page = 1, int pageSize = 20);

        Task<int> InsertGrade(int companyId, Grade grade);

        Task<int> UpdateGrade(int companyId, int id, Grade grade);

        Task<int> DeleteGrade(int companyId, int id);
    }
}