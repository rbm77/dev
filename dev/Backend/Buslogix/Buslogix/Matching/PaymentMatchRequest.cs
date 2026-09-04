namespace Buslogix.Matching
{
    /// <summary>
    /// Enqueued right after a payment_request or message_extraction_result
    /// row is successfully inserted, so PaymentMatchWorker can try to pair
    /// it against the other table by company_id + reference + amount.
    /// </summary>
    public record PaymentMatchRequest(int CompanyId, string Reference);
}
