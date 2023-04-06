using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Data
{
    public class TransactionsPool
    {
        public List<Transaction> Transactions { get; protected set; } = new List<Transaction>();

        public void AddTransaction( Transaction transaction )
        {
            ICryptoKeys cryptoKeys = new CryptoKeys();
            cryptoKeys.InitializeByXml(transaction.ReceiverWallet);
            IVerifier verifier = new Verifier();
            verifier.SetKeys(cryptoKeys);

            if (verifier.Verify(transaction))
            {
                if (Transactions.Contains(transaction))
                    return;
                Transactions.Add(transaction);
                Console.WriteLine("Someone sent valid transaction");
            }
            else
            {
                Console.WriteLine("Someone sent fake transaction");
            }
        }
    }
}
