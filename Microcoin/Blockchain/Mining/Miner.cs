﻿using Microcoin.Blockchain.Chain;
using Microcoin.Network.NodeNet;
using System.Threading;


namespace Microcoin.Blockchain.Mining
{
    public class Miner : IMiner
    {
        public MiningRules MiningRules { get; protected set; }

        public event Action<Block.Block, string> BlockMined;

        public void SetRules(MiningRules rules)
        {
            MiningRules = rules;
        }

        /// <summary>
        /// This is single-threaded, processor-based, not fast code.Since the project is educational, 
        /// this implementation will most likely be left, it shows a general example of how the blockchain works, 
        /// but provides an obvious way to perform a 50.1% network power attack.
        /// A person who implements a multi-threaded fast code will get a higher speed of hash generation and verification, 
        /// which will give him a huge increase in power against the background of other network members.
        /// Well, let's assume that everyone uses this version of the miner.
        /// </summary>
        public async Task<string> StartBlockMining(IChain chain, Block.Block block, string minerWallet, CancellationToken cancellationToken)
        {
            bool isBlockAlreadyMined = false;
            block.MiningBlockInfo.MinerPublicKey = minerWallet;
            Random random = new Random();
            while (cancellationToken.IsCancellationRequested is not true && isBlockAlreadyMined is not true )
            {
                int miningComplexity = MiningRules.ComplexityRule.Calculate(chain, block);
                block.MiningBlockInfo.Complexity = miningComplexity;
                decimal miningReward = MiningRules.RewardRule.Calculate(chain, block);
                block.MiningBlockInfo.MinerReward = miningReward;
                // To reduce count of complexity and reward recalculations
                for (int i = 0; i < 1024 * 16; i++)
                {
                    block.MiningBlockInfo.MinedValue = random.NextInt64() * (i % 2 == 1 ? 1 : -1);
                    var hash = block.GetMiningBlockHash();
                    if (Block.Block.GetHashComplexity(hash) < block.MiningBlockInfo.Complexity)
                        continue;
                    lock (this)
                    {
                        if (isBlockAlreadyMined is true)
                            break;
                        isBlockAlreadyMined = true;
                        cancellationToken.ThrowIfCancellationRequested();
                        BlockMined?.Invoke(block, hash);
                        return hash;
                    }
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            throw new Exception("Something wen't wrong");
        }

        public bool VerifyBlockMining(IChain chain, Block.Block block)
        {
            var isBlockRewardCorrect = MiningRules.RewardRule.Verify(chain, block);
            if (isBlockRewardCorrect is not true)
                return false;
            var isBlockComplexityCorrect = MiningRules.ComplexityRule.Verify(chain, block);
            if (isBlockComplexityCorrect is not true)
                return false;
            var computedHash = block.GetMiningBlockHash();
            if( computedHash != block.Hash)
                return false;
            if (Block.Block.GetHashComplexity(block.Hash) < block.MiningBlockInfo.Complexity)
                return false;
            return true;
        }
    }
}
