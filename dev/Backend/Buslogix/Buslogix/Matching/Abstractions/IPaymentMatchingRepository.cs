using Buslogix.Models.DTO;

namespace Buslogix.Matching.Abstractions
{
    public interface IPaymentMatchingRepository
    {
        /// <summary>
        /// Calls match_payment_request_extraction: tries to pair the
        /// payment_request and message_extraction_result rows sharing this
        /// company_id + reference. Matching does not care whether the
        /// payment_request was already validated - approval (which does
        /// require is_validated = 1) only runs when a match is actually
        /// found.
        /// </summary>
        Task<PaymentMatchResult> TryMatchAsync(int companyId, string reference);

        /// <summary>
        /// Calls match_payment_requests: a set-based sweep across every
        /// company that validates and unpairs everything currently
        /// matchable in one pass. Does not approve anything - that is
        /// auto_approve_payment_requests' job, run separately.
        /// </summary>
        Task<MatchSweepResult> MatchPendingPaymentRequests();
    }
}
