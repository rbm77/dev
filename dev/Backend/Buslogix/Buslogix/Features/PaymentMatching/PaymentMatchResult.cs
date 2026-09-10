namespace Buslogix.Features.PaymentMatching
{
    public record PaymentMatchResult(bool Matched, bool AmountMismatch, int? PaymentRequestId, long NewPaymentId);
}
