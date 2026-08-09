using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class ContactService(IContactRepository contactRepository) : IContactService
    {

        public async Task<PagedResult<Contact>> GetContacts(int companyId, int studentId, int page = 1, int pageSize = 20)
        {
            return await contactRepository.GetContacts(companyId, studentId, page, pageSize);
        }

        public async Task<int> InsertContact(int companyId, int studentId, Contact contact)
        {
            return await contactRepository.InsertContact(companyId, studentId, contact);
        }

        public async Task<bool> UpdateContact(int companyId, int studentId, int id, Contact contact)
        {
            int affected = await contactRepository.UpdateContact(companyId, studentId, id, contact);
            return affected > 0;
        }

        public async Task<bool> DeleteContact(int companyId, int studentId, int id)
        {
            int affected = await contactRepository.DeleteContact(companyId, studentId, id);
            return affected > 0;
        }
    }
}