namespace Buslogix.Matching
{
    /// <summary>Outcome of a single match_payment_request_extraction call.</summary>
    public record PaymentMatchResult(bool Matched, bool AmountMismatch, int? PaymentRequestId, long NewPaymentId);
}
