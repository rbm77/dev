using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IContactService
    {

        Task<PagedResult<Contact>> GetContacts(int companyId, int studentId, int page = 1, int pageSize = 20);

        Task<int> InsertContact(int companyId, int studentId, Contact contact);

        Task<bool> UpdateContact(int companyId, int studentId, int id, Contact contact);

        Task<bool> DeleteContact(int companyId, int studentId, int id);
    }
}

