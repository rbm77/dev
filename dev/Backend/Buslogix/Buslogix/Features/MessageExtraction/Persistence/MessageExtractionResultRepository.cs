using System.Data;
using Buslogix.Features.MessageExtraction.Abstractions;
using MySqlConnector;

namespace Buslogix.Features.MessageExtraction.Persistence
{
    public class MessageExtractionResultRepository(IDataAccess dataAccess) : IMessageExtractionResultRepository
    {
        public async Task Insert(int companyId, ExtractedData data)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_amount"] = data.Amount,
                ["p_reference"] = data.Reference,
                ["p_date"] = data.Date
            };

            object? result;
            try
            {
                result = await dataAccess.ExecuteScalar("insert_message_extraction_result", CommandType.StoredProcedure, parameters);
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                throw new DuplicateReferenceException(data.Reference, ex);
            }

            if (result is not null && Convert.ToInt64(result) == -1)
            {
                throw new DuplicateReferenceException(data.Reference);
            }
        }
    }
}
