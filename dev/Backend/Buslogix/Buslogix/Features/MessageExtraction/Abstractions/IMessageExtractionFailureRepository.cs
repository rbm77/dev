namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageExtractionFailureRepository
    {
        Task Insert(int companyId, string rawText);

        Task<List<ClaimedExtractionFailure>> ClaimAllForRetry();
    }
}
