using Chain;

namespace ChainController
{
    public interface IDeepTransactionsVerify
    {
        public Task<bool> Verify(AbstractChain chain, List<Transaction.Transaction> transactions, CancellationToken cancellationToken);
    }
}
