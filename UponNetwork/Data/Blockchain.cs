using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microcoin;

namespace Microcoin.Data
{
    public class Blockchain
    {
        public Peer.Peer Peer { get; protected set; }
        public List<Block> Blocks {  get; protected set; }
        
        public Blockchain(Peer.Peer parentPeer)
        {
            Peer = parentPeer;
            Blocks = new List<Block>();
        }

        public decimal CalculateWalletCoins(string walletPublicKey)
        {
            decimal coins = 0;
            int blockCount = Blocks.Count;
            for( int i = 0; i < Blocks.Count; i++)
            {
                var block = Blocks[i];
                if (block.MinerWallet == walletPublicKey)
                    coins += block.MiningReward;

                var transactions = block.Transactions;
                foreach( Transaction transaction in transactions ) {
                    if (transaction.SenderWallet == walletPublicKey)
                        coins -= transaction.CoinsToSend;
                    else if( transaction.ReceiverWallet == walletPublicKey )
                        coins += transaction.CoinsToSend;
                }
            }
            return coins;
        }

        // Forse initiate of blockchain
        // by creating first block 
        // here you can give yourself billions of coins :)
        public static Block CreateInitialBlock( string begginerWallet )
        {
            Transaction transaction = new Transaction();
            transaction.CoinsToSend = 1000000;
            transaction.ReceiverWallet = begginerWallet;
            transaction.SenderWallet = "";
            transaction.Signature = "";

            Block block = new Block();
            block.Id = 0;
            block.MiningReward = 0;
            block.MagikValue = 0;
            block.CreationTime = new DateTime(2023, 03, 13);
            block.Transactions = new List<Transaction>();
            block.Transactions.Add(transaction);

            return block;
        }

        public Block CreateBlockForMining(List<Transaction> transactions)
        {
            var prewBlock = Blocks[^1];

            Block block = new Block();
            block.Transactions = transactions;
            block.MiningReward = 10;
            block.MinerWallet = ((CryptoKeys)Peer.CryptoKeys).PublicKeyXml;
            block.PrewBlockHash = Convert.ToBase64String(prewBlock.BlockHash());
            block.CreationTime = DateTime.UtcNow;
            block.Id = Blocks.Count;
            Blocks.Add(block);

            return block;
        }

        public void CreateNewBlock(TransactionsPool transactionsPool)
        {
            
        }
    }
}
