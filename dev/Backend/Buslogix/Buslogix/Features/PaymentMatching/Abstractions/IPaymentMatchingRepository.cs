
namespace Buslogix.Features.PaymentMatching.Abstractions
{
    public interface IPaymentMatchingRepository
    {
        Task<PaymentMatchResult> TryMatch(int companyId, string reference);

        Task<MatchSweepResult> MatchPendingPaymentRequests();
    }
}
