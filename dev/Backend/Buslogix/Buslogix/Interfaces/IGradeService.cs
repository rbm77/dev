using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IGradeService
    {

        Task<Grade?> GetGrade(int companyId, int id);

        Task<PagedResult<Grade>> GetGrades(int companyId, string? description = null, int page = 1, int pageSize = 20);

        Task<int> InsertGrade(int companyId, Grade grade);

        Task<bool> UpdateGrade(int companyId, int id, Grade grade);

        Task<bool> DeleteGrade(int companyId, int id);
    }
}
