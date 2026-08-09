using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IContactRepository
    {

        Task<PagedResult<Contact>> GetContacts(int companyId, int studentId, int page = 1, int pageSize = 20);

        Task<int> InsertContact(int companyId, int studentId, Contact contact);

        Task<int> UpdateContact(int companyId, int studentId, int id, Contact contact);

        Task<int> DeleteContact(int companyId, int studentId, int id);
    }
}
