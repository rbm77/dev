using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IContactService
    {

        Task<List<Contact>> GetContacts(int companyId, int studentId);

        Task<int> InsertContact(int companyId, int studentId, Contact contact);

        Task<bool> UpdateContact(int companyId, int studentId, int id, Contact contact);

        Task<bool> DeleteContact(int companyId, int studentId, int id);
    }
}

