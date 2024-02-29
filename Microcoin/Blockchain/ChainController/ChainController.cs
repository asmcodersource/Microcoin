﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microcoin.Blockchain.Chain;
using Microcoin.Blockchain.Mining;

namespace Microcoin.Blockchain.ChainController
{
    public class ChainController
    {   
        protected CancellationTokenSource currentChainOperationsCTS;
        public ChainLoader ChainLoader { get; protected set; }
        public Chain.Chain ChainTail { get; protected set; }
        public IMiner Miner { get; protected set; }
        public INextBlockRule NextBlockRule { get; protected set; }
        public IDeepTransactionsVerify DeepTransactionsVerify { get; protected set; }
        public IFetchableChainRule FetchableChainRule { get; protected set; }

        public ChainController(Chain.Chain chainTail, IMiner miner, ChainLoader chainLoader = null)
        {
            // ChainControllers without chainLoader can't load chain from network
            // This is done to avoid loading threads inside loading threads.
            ChainTail = chainTail;
            ChainLoader = chainLoader;
            Miner = miner;
            currentChainOperationsCTS = new CancellationTokenSource();
        }

        public async Task AcceptBlock(Block.Block block)
        {
            CancellationToken cancellationToken = GetChainOperationCancellationToken();
            if (NextBlockRule.IsBlockNextToChain(block, ChainTail))
            {
                try
                {
                    var isBlockValid = await DeepBlockVerify(block, ChainTail, cancellationToken);
                    if (isBlockValid is not true)
                        return; 
                    AddBlockToTail(block);  // this block valid as new tail of current blockchain, try to set it as tail
                }
                catch (TaskCanceledException) { /* This block did not have time to pass the inspection, most likely another block was accepted as the final one. */ }
            }
            else if (ChainLoader != null && FetchableChainRule.IsPossibleChainUpgrade(ChainTail, block))
            {
                // Essentially, with this call we only request that the chain be loaded, but not necessarily that it will be loaded.
                // We don't care about everything else inside the chain.
                ChainLoader.RequestChainFetch(block);
            }
        }

        public void AddBlockToTail(Block.Block block)
        {
            // lock used to avoid thread running
            // Only one block can be added as tail of chain
            lock (ChainTail)
            {
                // is block still continues to this chain?
                if (NextBlockRule.IsBlockNextToChain(block, ChainTail) is not true)
                    return;
                ChainTail.AddTailBlock(block);
                currentChainOperationsCTS.Cancel();
                currentChainOperationsCTS = new CancellationTokenSource();
            }
        }

        protected async Task<bool> DeepBlockVerify(Block.Block block, IChain chainTail, CancellationToken cancellationToken)
        {
            // verify block is corret itself
            var isShortBlockValid = await ShortBlockVerify(block, chainTail, cancellationToken);
            if (isShortBlockValid is not true)
                return false;
            // Verify each transactions to have correct count of coins
            var isTransactionsCorrect = await DeepTransactionsVerify.Verify(chainTail, block.Transactions, cancellationToken);
            if (isTransactionsCorrect is not true)
                return false;
            return true;
        } 

        protected async Task<bool> ShortBlockVerify(Block.Block block, IChain chainTail, CancellationToken cancellationToken)
        {
            lock (ChainTail)
            {
                // It would work, and prevent change of chain while miner working,
                // but it take to much time, and memory space
                // TODO: Think about non immutable chain, or one non immutable chain for all handled blocks 
                cancellationToken.ThrowIfCancellationRequested();
                var immutalbeChain = new ImmutableChain(ChainTail);
                return Miner.VerifyBlockMining(chainTail, block).Result;
            }
        }

        // Any actions that start for some Chain valid only for this chain
        // If some operation change current chain, all operations have to be cancelled
        protected CancellationToken GetChainOperationCancellationToken()
        {
            // I used lock to prevent returning past CTS
            lock (ChainTail)
                return currentChainOperationsCTS.Token;
        }
    }
}
