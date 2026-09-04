using System.Data;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using MySqlConnector;

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

            object? result;
            try
            {
                result = await dataAccess.ExecuteScalar("insert_message_extraction_result", CommandType.StoredProcedure, parameters);
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                // Lost a race against a concurrent insert of the same reference:
                // both passed insert_message_extraction_result's own pre-check
                // before either committed, so the UNIQUE KEY on `reference` is
                // what actually caught it here.
                throw new DuplicateReferenceException(data.Reference, ex);
            }

            if (result is not null && Convert.ToInt64(result) == -1)
            {
                // insert_message_extraction_result's pre-check found the
                // reference already in message_extraction_result or
                // payment_receipt_reference - nothing was inserted.
                throw new DuplicateReferenceException(data.Reference);
            }
        }
    }
}
