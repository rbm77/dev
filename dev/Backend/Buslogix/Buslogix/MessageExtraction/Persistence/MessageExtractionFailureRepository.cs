using System.Data;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.Utilities;

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

        public async Task<List<ClaimedExtractionFailure>> ClaimAllForRetryAsync()
        {
            return await dataAccess.ExecuteReader("retry_message_extraction_failures", CommandType.StoredProcedure,
                static reader => new ClaimedExtractionFailure(
                    reader.GetInt32OrDefault(0),
                    reader.GetStringOrDefault(1) ?? string.Empty,
                    reader.GetDateTimeOrDefault(2) ?? DateTime.UtcNow),
                null);
        }
    }
}
