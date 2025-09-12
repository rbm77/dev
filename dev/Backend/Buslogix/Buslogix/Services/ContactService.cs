using Buslogix.Interfaces;
using Buslogix.Models;

namespace Buslogix.Services
{
    public class ContactService(IContactRepository contactRepository) : IContactService
    {

        private readonly IContactRepository _contactRepository = contactRepository;

        public async Task<List<Contact>> GetContacts(int companyId, int studentId)
        {
            return await _contactRepository.GetContacts(companyId, studentId);
        }

        public async Task<int> InsertContact(int companyId, int studentId, Contact contact)
        {
            return await _contactRepository.InsertContact(companyId, studentId, contact);
        }

        public async Task<bool> UpdateContact(int companyId, int studentId, int id, Contact contact)
        {
            int affected = await _contactRepository.UpdateContact(companyId, studentId, id, contact);
            return affected > 0;
        }

        public async Task<bool> DeleteContact(int companyId, int studentId, int id)
        {
            int affected = await _contactRepository.DeleteContact(companyId, studentId, id);
            return affected > 0;
        }
    }
}