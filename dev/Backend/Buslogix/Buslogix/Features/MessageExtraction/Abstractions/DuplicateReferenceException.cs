namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public class DuplicateReferenceException(string reference, Exception? innerException = null)
        : Exception($"Reference '{reference}' already exists.", innerException)
    {
        public string Reference { get; } = reference;
    }
}
