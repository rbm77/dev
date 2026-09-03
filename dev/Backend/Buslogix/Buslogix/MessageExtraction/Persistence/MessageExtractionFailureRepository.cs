using System.Data;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;

namespace Buslogix.MessageExtraction.Persistence
{
    public class MessageExtractionFailureRepository(IDataAccess dataAccess) : IMessageExtractionFailureRepository
    {
        public async Task InsertAsync(int companyId, string rawText)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_raw_text"] = rawText
            };

            await dataAccess.ExecuteScalar("insert_message_extraction_failure", CommandType.StoredProcedure, parameters);
        }
    }
}
