using Chain;

namespace Mining
{
    /// <summary>
    /// This rule determines the difficulty of the next block. 
    /// The complexity of the block determines the number of calculations that the network must perform to mine the block. 
    /// The idea was taken from the Bitcoin network. Every 2048 blocks, a difficulty check is performed; 
    /// if mining lasts more than 10 minutes, the difficulty decreases; 
    /// if it takes less, it increases.
    /// </summary>
    public class ComplexityRule : IComplexityRule
    {
        protected int defaultComplexity = 4;
        protected int targetTime = 10;
        protected int allowedTimeDivitation = 3;
        protected int avgWindow = 10;

        public int Calculate(AbstractChain contextChain, Block.Block block)
        {
            var windowFirstBlock = contextChain.GetBlockFromTail(2048);
            var windowLastBlock = contextChain.GetBlockFromTail(0);
            if (windowFirstBlock is null)
                return defaultComplexity;
            var durationWindow = windowFirstBlock.MiningBlockInfo.CreateTime - windowLastBlock.MiningBlockInfo.CreateTime;
            var duration = durationWindow / 2048;
            if (Math.Abs(duration.Minutes - targetTime) < allowedTimeDivitation)
            {
                return windowLastBlock.MiningBlockInfo.Complexity;
            }
            else
            {
                if (duration.Minutes - targetTime >= 0)
                    return windowLastBlock.MiningBlockInfo.Complexity--;
                else
                    return windowLastBlock.MiningBlockInfo.Complexity++;
            }
        }

        public bool Verify(AbstractChain contextChain, Block.Block block)
        {
            var complexity = Calculate(contextChain, block);
            if (block.MiningBlockInfo.Complexity < complexity)
                return false;
            return true;
        }
    }
}
