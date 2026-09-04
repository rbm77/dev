using System.Data;
using Buslogix.Interfaces;
using Buslogix.Matching.Abstractions;
using Buslogix.Models.DTO;
using Buslogix.Utilities;

namespace Buslogix.Matching.Persistence
{
    public class PaymentMatchingRepository(IDataAccess dataAccess) : IPaymentMatchingRepository
    {
        public async Task<PaymentMatchResult> TryMatchAsync(int companyId, string reference)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_reference"] = reference
            };

            Dictionary<string, DbType> outputParameters = new()
            {
                ["p_matched"] = DbType.Int32,
                ["p_amount_mismatch"] = DbType.Int32,
                ["p_payment_request_id"] = DbType.Int32,
                ["p_new_payment_id"] = DbType.Int64
            };

            (_, IDictionary<string, object?> output) = await dataAccess.ExecuteNonQuery(
                "match_payment_request_extraction", CommandType.StoredProcedure, parameters, outputParameters);

            bool matched = Convert.ToInt32(output["p_matched"] ?? 0) == 1;
            bool amountMismatch = Convert.ToInt32(output["p_amount_mismatch"] ?? 0) == 1;
            int? paymentRequestId = output["p_payment_request_id"] is null ? null : Convert.ToInt32(output["p_payment_request_id"]);
            long newPaymentId = Convert.ToInt64(output["p_new_payment_id"] ?? 0L);

            return new PaymentMatchResult(matched, amountMismatch, paymentRequestId, newPaymentId);
        }

        public async Task<MatchSweepResult> MatchPendingPaymentRequests()
        {
            List<MatchSweepResult> rows = await dataAccess.ExecuteReader("match_payment_requests", CommandType.StoredProcedure,
                static reader => new MatchSweepResult
                {
                    MatchedCount = reader.GetInt32OrDefault(0)
                }, null);

            return rows.Count > 0 ? rows[0] : new MatchSweepResult();
        }
    }
}
