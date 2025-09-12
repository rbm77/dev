using Buslogix.Models;

namespace Buslogix.Interfaces
{
    public interface IContactRepository
    {

        Task<List<Contact>> GetContacts(int companyId, int studentId);

        Task<int> InsertContact(int companyId, int studentId, Contact contact);

        Task<int> UpdateContact(int companyId, int studentId, int id, Contact contact);

        Task<int> DeleteContact(int companyId, int studentId, int id);
    }
}
