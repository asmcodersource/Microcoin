using Microcoin.Blockchain.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Mining
{
    public interface IMiner
    {
        public event Action<Block.Block, string> BlockMined;
        public MiningRules MiningRules { get; }
        
        public void LinkBlockToChain(AbstractChain chain, Block.Block block)
        {
            var tailBlock = chain.GetLastBlock();
            if (tailBlock is not null)
            {
                block.MiningBlockInfo.BlockId = tailBlock.MiningBlockInfo.BlockId + 1;
                block.MiningBlockInfo.PreviousBlockHash = tailBlock.Hash;
            }
            else throw new Exception("Can't link to empty chain");
        }

        public Task<string> StartBlockMining(AbstractChain chain, Block.Block block, string minerWallet, CancellationToken cancellationToken);
        public bool VerifyBlockMining(AbstractChain chain, Block.Block block);
    }
}
