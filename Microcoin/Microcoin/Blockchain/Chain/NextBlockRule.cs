namespace Chain
{
    public class NextBlockRule : INextBlockRule
    {
        public bool IsBlockNextToChain(Block.Block block, AbstractChain chain)
        {
            var currentTailBlock = chain.GetLastBlock();
            if (currentTailBlock.MiningBlockInfo.BlockId + 1 != block.MiningBlockInfo.BlockId)
                return false;
            if (currentTailBlock.Hash != block.MiningBlockInfo.PreviousBlockHash)
                return false;
            return true;
        }
    }
}
