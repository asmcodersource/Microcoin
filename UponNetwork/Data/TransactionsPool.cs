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
        public Blockchain Blockchain { get; protected set; }
        public List<Transaction> Transactions { get; protected set; } = new List<Transaction>();

        public TransactionsPool(Blockchain blockchain)
        {
            this.Blockchain = blockchain;
        }

        public void AddTransaction( Transaction transaction )
        {
            ICryptoKeys cryptoKeys = new CryptoKeys();
            cryptoKeys.InitializeByXml(transaction.SenderWallet);
            IVerifier verifier = new Verifier();
            verifier.SetKeys(cryptoKeys);

            // Verify transaction for correct
            if (verifier.Verify(transaction) == false || Transactions.Contains(transaction))
                return;

            decimal senderWalletCoins = 0;
            senderWalletCoins = Blockchain.CalculateWalletCoins(transaction.SenderWallet);
            if (senderWalletCoins < transaction.CoinsToSend)
                return;

            lock (Transactions)
                Transactions.Add(transaction);
            Console.WriteLine("Someone sent valid transaction");
        }
    }
}
