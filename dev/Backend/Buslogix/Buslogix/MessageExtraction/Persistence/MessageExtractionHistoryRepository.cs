using System.Data;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.Utilities;

namespace Buslogix.MessageExtraction.Persistence
{
    public class MessageExtractionHistoryRepository(IDataAccess dataAccess) : IMessageExtractionHistoryRepository
    {
        public async Task<MessageExtractionPurgeResult> PurgeExpiredRecordsAsync()
        {
            List<MessageExtractionPurgeResult> rows = await dataAccess.ExecuteReader("purge_message_extraction_history", CommandType.StoredProcedure,
                static reader => new MessageExtractionPurgeResult(
                    reader.GetInt32OrDefault(0),
                    reader.GetInt32OrDefault(1)),
                null);

            return rows.Count > 0 ? rows[0] : new MessageExtractionPurgeResult(0, 0);
        }
    }
}
