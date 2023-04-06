using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Data
{
    public class Blockchain
    {
        public List<Block> Blocks {  get; set; }
        
        public Blockchain()
        {
            Blocks = new List<Block>();

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
            block.Signature = "";
            block.MiningReward = 0;
            block.MagikValue = new byte[0];
            block.CreationTime = new DateTime(2023, 03, 13);
            block.Transactions = new List<Transaction>();
            block.Transactions.Add(transaction);

            return block;
        }
    }
}
