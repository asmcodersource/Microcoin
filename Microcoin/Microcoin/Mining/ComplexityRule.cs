﻿using Microcoin.Microcoin.Blockchain.Chain;
using Microcoin.Microcoin.Blockchain.Block;

namespace Microcoin.Microcoin.Mining
{
    /// <summary>
    /// This rule determines the difficulty of the next block. 
    /// The complexity of the block determines the number of calculations that the network must perform to mine the block. 
    /// The idea was taken from the Bitcoin network. Every 'avgWindow' blocks, a difficulty check is performed; 
    /// if mining lasts more than 'targetTime' minutes, the difficulty decreases; 
    /// if it takes less, it increases.
    /// </summary>
    public class ComplexityRule : IComplexityRule
    {
        protected int defaultComplexity = 19;
        protected int targetTime = 2;
        protected int allowedTimeDivitation = 1;
        protected int avgWindow = 5;

        public int Calculate(AbstractChain contextChain, Block block)
        {
            var windowLastBlock = contextChain.GetBlockFromTail(0);
            if (windowLastBlock == null)
                return defaultComplexity;
            var windowFirstBlock = contextChain.GetBlockFromTail(avgWindow);
            if (windowFirstBlock == null || windowFirstBlock == windowLastBlock)
                windowFirstBlock = contextChain.GetBlockFromTail(windowLastBlock.MiningBlockInfo.BlockId);
            if (windowFirstBlock == null || windowFirstBlock == windowLastBlock)
                return defaultComplexity;
            var durationWindow =  windowLastBlock.MiningBlockInfo.CreateTime - windowFirstBlock.MiningBlockInfo.CreateTime;
            var actualWindowSize = windowLastBlock.MiningBlockInfo.BlockId - windowFirstBlock.MiningBlockInfo.BlockId;
            var duration = durationWindow / actualWindowSize;
            if (Math.Abs(duration.TotalMinutes - targetTime) < allowedTimeDivitation)
            {
                return windowLastBlock.MiningBlockInfo.Complexity;
            }
            else
            {
                if (duration.Minutes - targetTime >= 0)
                    return windowLastBlock.MiningBlockInfo.Complexity - 1;
                else
                    return windowLastBlock.MiningBlockInfo.Complexity + 1;
            }
        }

        public bool Verify(AbstractChain contextChain, Block block)
        {
            var complexity = Calculate(contextChain, block);
            if (block.MiningBlockInfo.Complexity < complexity)
                return false;
            return true;
        }
    }
}
