using System.Data;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;

namespace Buslogix.MessageExtraction.Persistence
{
    public class MessageExtractionResultRepository(IDataAccess dataAccess) : IMessageExtractionResultRepository
    {
        public async Task InsertAsync(int companyId, ExtractedData data)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_amount"] = data.Amount,
                ["p_reference"] = data.Reference,
                ["p_date"] = data.Date
            };

            await dataAccess.ExecuteScalar("insert_message_extraction_result", CommandType.StoredProcedure, parameters);
        }
    }
}
