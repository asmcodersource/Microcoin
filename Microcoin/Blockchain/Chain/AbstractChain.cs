using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Chain
{
    /// <summary>
    /// The chain is a sequence of blocks for which the connection rule is executed. 
    /// A chain can be connected to the end of another chain. 
    /// A chain acting as a parent for another must be immutable
    /// </summary>
    [Serializable]
    public abstract class AbstractChain
    {
        public Dictionary<string, double> WalletsCoins { get; protected set; }
        public ImmutableChain? PreviousChain { get; protected set; }
        public HashSet<Transaction.Transaction> TransactionsSet { get; protected set; }
        public Dictionary<string, Block.Block> BlocksDictionary { get; protected set; }
        protected List<Block.Block> blocksList { get; set; }

        public Block.Block? GetBlockFromBegin(int blockIdFromBegin)
        {
            if (blocksList.Count <= blockIdFromBegin)
                return null;
            return blocksList[blockIdFromBegin];
        }

        public Block.Block? GetBlockFromTail(int blockIdFromTail)
        {
            if (GetLastBlock() is Block.Block tailBlock)
            {
                int lastBlockId = tailBlock.MiningBlockInfo.BlockId;
                return GetBlock(lastBlockId - blockIdFromTail);
            }
            return null;
        }

        public Block.Block? GetBlock(int blockId)
        {
            var currentChain = this;
            if (currentChain.blocksList.Count() == 0)
                return null;
            while (currentChain is not null)
            {
                if (currentChain.GetLastBlock().MiningBlockInfo.BlockId > blockId)
                    if (currentChain.GetFirstBlock().MiningBlockInfo.BlockId <= blockId)
                        return currentChain.blocksList[blockId - currentChain.GetFirstBlock().MiningBlockInfo.BlockId];
                currentChain = currentChain.PreviousChain;
            }
            return null;
        }

        public List<Block.Block> GetBlocksList()
            => blocksList;

        public void SetBlockList(List<Block.Block> blockList)
            => this.blocksList = blockList;

        public bool IsChainHasTransaction(Transaction.Transaction transaction)
            => TransactionsSet.Contains(transaction);

        public double GetWalletCoins(string walletPublicKey)
            => WalletsCoins.ContainsKey(walletPublicKey) ? WalletsCoins[walletPublicKey] : 0;

        public Block.Block? GetFirstBlock()
            => blocksList.Count() == 0 ? null : blocksList.First();

        public Block.Block? GetLastBlock() 
            => blocksList.Count() == 0 ? null : blocksList.Last();

    }
}
