using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Data
{
    public class TransactionsPool
    {
        public Blockchain Blockchain { get; protected set; }
        public List<Transaction> Transactions { get; protected set; }
        public Dictionary<string, decimal> PoolCoinsBalance { get; protected set; }

        public TransactionsPool(Blockchain blockchain)
        {
            this.PoolCoinsBalance = new Dictionary<string, decimal>();
            this.Transactions = new List<Transaction>();
            this.Blockchain = blockchain;
        }

        public void AddTransaction( Transaction transaction )
        {

            lock (this)
            {
                // Verify transaction for correct
                if (transaction.IsTransactionCorrect() == false || Transactions.Contains(transaction))
                    return;


                if (PoolCoinsBalance.ContainsKey(transaction.SenderWallet) == false)
                    PoolCoinsBalance.Add(transaction.SenderWallet, 0);

                decimal senderWalletCoins = PoolCoinsBalance[transaction.SenderWallet];
                senderWalletCoins += Blockchain.CalculateWalletCoins(transaction.SenderWallet);
                //if (senderWalletCoins < transaction.CoinsToSend)
                //    return;

                PoolCoinsBalance[transaction.SenderWallet] = -transaction.CoinsToSend;
                if (PoolCoinsBalance.ContainsKey(transaction.ReceiverWallet) == false)
                    PoolCoinsBalance.Add(transaction.ReceiverWallet, 0);
                PoolCoinsBalance[transaction.ReceiverWallet] = +transaction.CoinsToSend;
                Transactions.Add(transaction);
                NewTransactionConfirmed?.Invoke(this, null);
                Console.WriteLine(transaction.ToString());
            }
        }

        public Block ClaimNextBlock()
        {
            var comparer = new TransactionSortComparer();
            var transactions = this.Transactions;
            Block block = new Block();

            lock (this)
            {
                this.Transactions.Sort(comparer);
                transactions = this.Transactions;
                this.PoolCoinsBalance.Clear();
                this.Transactions = new List<Transaction>();

                block = Blockchain.CreateBlockForMining(transactions);
                return block;
            }
        }

        public event EventHandler NewTransactionConfirmed;
    }


    class TransactionSortComparer : IComparer<Transaction>
    {
        public int Compare(Transaction? x, Transaction? y)
        {
            return (int)(x.CreationTime.Millisecond - y.CreationTime.Millisecond);

        }
    }
}
