using Chain;
using ChainController;

namespace TransactionsPool
{
    /// <summary>
    /// To create a new block and start mining it, you need to get the transactions of this block. 
    /// A block's transactions must be suitable for a particular chain, 
    /// so deep validation should be invoked to determine which transactions should be removed from the pool 
    /// and which should go into the new block. 
    /// This is an important part of the blockchain, I will review the correctness of my choice later.
    /// </summary>
    public static class TailTransactionsClaimer
    {
        public static List<Transaction.Transaction> ClaimTailTransactions(this TransactionsPool transactionsPool, AbstractChain chain, IDeepTransactionsVerify deepTransactionsVerify, int maxTransactionsCount)
        {
            var poolTransactions = transactionsPool.TakeTransactions();
            var blockTransactions = new List<Transaction.Transaction>();
            var removeTransations = new List<Transaction.Transaction>();
            foreach (var transaction in poolTransactions)
            {
                if (blockTransactions.Count > maxTransactionsCount)
                    break;
                blockTransactions.Add(transaction);
                removeTransations.Add(transaction);
                var isSuccess = deepTransactionsVerify.Verify(chain, blockTransactions, CancellationToken.None).Result;
                if (isSuccess is not true)
                    blockTransactions.Remove(transaction);
            }
            transactionsPool.RemoveTransactions(removeTransations);
            return blockTransactions;
        }
    }
}
