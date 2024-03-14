using Microcoin.Blockchain.Chain;

namespace Microcoin.Blockchain.Mining
{
    public interface IMiner
    {
        public event Action<Microcoin.Blockchain.Block.Block, string> BlockMined;
        public MiningRules MiningRules { get; }

        public void LinkBlockToChain(AbstractChain chain, Microcoin.Blockchain.Block.Block block)
        {
            var tailBlock = chain.GetLastBlock();
            if (tailBlock is not null)
            {
                block.MiningBlockInfo.BlockId = tailBlock.MiningBlockInfo.BlockId + 1;
                block.MiningBlockInfo.PreviousBlockHash = tailBlock.Hash;
            }
            else throw new Exception("Can't link to empty chain");
        }

        public Task<string> StartBlockMining(AbstractChain chain, Microcoin.Blockchain.Block.Block block, string minerWallet, CancellationToken cancellationToken);
        public bool VerifyBlockMining(AbstractChain chain, Microcoin.Blockchain.Block.Block block);
    }
}
