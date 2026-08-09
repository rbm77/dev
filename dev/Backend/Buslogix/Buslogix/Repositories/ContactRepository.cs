using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class ContactRepository(IDataAccess dataAccess) : IContactRepository
    {

        public async Task<PagedResult<Contact>> GetContacts(int companyId, int studentId, int page = 1, int pageSize = 20)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = studentId,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<Contact> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_contacts", CommandType.StoredProcedure,
                static reader => new Contact
                {
                    Id = reader.GetInt32OrDefault(0),
                    PhoneNumber = reader.GetStringOrDefault(1),
                    Description = reader.GetStringOrDefault(2),
                    IsActive = reader.GetBooleanOrDefault(3)
                }, parameters);

            return new PagedResult<Contact>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> InsertContact(int companyId, int studentId, Contact contact)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = studentId,
                ["p_phone_number"] = contact.PhoneNumber,
                ["p_description"] = contact.Description,
                ["p_is_active"] = contact.IsActive
            };

            object? result = await dataAccess.ExecuteScalar("insert_contact", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<int> UpdateContact(int companyId, int studentId, int id, Contact contact)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = studentId,
                ["p_id"] = id,
                ["p_phone_number"] = contact.PhoneNumber,
                ["p_description"] = contact.Description,
                ["p_is_active"] = contact.IsActive
            };

            return await dataAccess.ExecuteNonQuery("update_contact", CommandType.StoredProcedure, parameters);
        }

        public async Task<int> DeleteContact(int companyId, int studentId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_student_id"] = studentId,
                ["p_id"] = id
            };

            return await dataAccess.ExecuteNonQuery("delete_contact", CommandType.StoredProcedure, parameters);
        }
    }
}