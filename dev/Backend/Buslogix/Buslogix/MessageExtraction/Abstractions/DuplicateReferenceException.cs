namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// Thrown by IMessageExtractionResultRepository.InsertAsync when
    /// data.Reference already exists in message_extraction_result or in
    /// payment_receipt_reference - not a real failure, just a message that
    /// was already processed/confirmed elsewhere. Callers should log and
    /// move on, not route it to message_extraction_failure.
    /// </summary>
    public class DuplicateReferenceException(string reference, Exception? innerException = null)
        : Exception($"Reference '{reference}' already exists.", innerException)
    {
        public string Reference { get; } = reference;
    }
}
