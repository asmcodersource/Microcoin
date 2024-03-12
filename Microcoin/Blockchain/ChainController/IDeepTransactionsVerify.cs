using Microcoin.Blockchain.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.ChainController
{
    public interface IDeepTransactionsVerify
    {
        public Task<bool> Verify(AbstractChain chain, List<Transaction.Transaction> transactions, CancellationToken cancellationToken);
    }
}
