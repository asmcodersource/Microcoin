﻿using Microcoin.Blockchain.Chain;

namespace Microcoin.Blockchain.Mining
{
    public class RewardRule : IRewardRule
    {
        public double Calculate(AbstractChain contextChain, Microcoin.Blockchain.Block.Block block)
        {
            return CalculateRewardOfBlock(block);
        }

        public bool Verify(AbstractChain contextChain, Microcoin.Blockchain.Block.Block block)
        {
            double reward = CalculateRewardOfBlock(block);
            if (block.MiningBlockInfo.MinerReward != reward)
                return false;
            return true;
        }

        protected double CalculateRewardOfBlock(Microcoin.Blockchain.Block.Block block)
        {
            return block.MiningBlockInfo.Complexity * (1.0 / (block.MiningBlockInfo.BlockId + 1));
        }
    }
}