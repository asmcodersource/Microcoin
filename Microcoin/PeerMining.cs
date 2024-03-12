using Microcoin.Blockchain.Block;
using Microcoin.Blockchain.Chain;
using Microcoin.Blockchain.ChainController;
using Microcoin.Blockchain.Mining;
using Microcoin.Blockchain.TransactionsPool;
using System.Threading;

namespace Microcoin
{
    public class PeerMining
    {
        public event Action<Block> BlockMined;
        protected Task? MiningTask;
        protected CancellationTokenSource? MiningCancellationTokenSource;
        public int MaxTransactionsPerBlock { get; set; } = 512;
        public IMiner Miner { get; protected set; } 
        public TransactionsPool LinkedTransactionsPool { get; protected set; }
        public bool IsMiningEnabled { get; protected set; } = false;
        public string MinerWaller { get; set; }

        public void InizializeMiner(IMiner miner, string minerWallet, TransactionsPool transactionsPool)
        {
            Miner = miner;
            MinerWaller = minerWallet;
            LinkedTransactionsPool = transactionsPool;
        }

        public void StartMining()
        {
            IsMiningEnabled = true;
        }

        public void StopMining()
        {
            IsMiningEnabled = false;
            CancelCurrentMiningProcess();
        }

        /// <returns>true if some mining process was cancelled</returns>
        public bool CancelCurrentMiningProcess()
        {
            lock (this)
            {
                if (MiningCancellationTokenSource is not null)
                {
                    MiningCancellationTokenSource?.Cancel();
                    MiningTask?.Wait();
                    return true;
                }
                return false;
            }
        }

        public void TryStartMineBlock(AbstractChain tailChain, IDeepTransactionsVerify deepTransactionsVerify)
        {
            lock (this)
            {
                if (MiningTask is not null && MiningTask.IsCompleted is not true)
                    throw new Exception("Only one mining proccess allowed");

                if (IsMiningEnabled)
                {
                    MiningCancellationTokenSource = new CancellationTokenSource();
                    StartMineBlock(tailChain, deepTransactionsVerify, MiningCancellationTokenSource.Token);
                }
            }
        }
        
        public void StartMineBlock(AbstractChain tailChain, IDeepTransactionsVerify deepTransactionsVerify, CancellationToken cancellationToken)
        {
            var transactions = LinkedTransactionsPool.ClaimTailTransactions(tailChain, deepTransactionsVerify, MaxTransactionsPerBlock);
            if (transactions.Count == 0)
                return;
            Block block = new Block();
            block.MiningBlockInfo = new MiningBlockInfo();
            block.Transactions = transactions;
            Miner.LinkBlockToChain(tailChain, block);
            // TODO: verify this line
            MiningTask = Task.Run(() => MineBlock(tailChain, block, deepTransactionsVerify, cancellationToken));
        }
 

        protected async Task MineBlock(AbstractChain tailChain, Block block, IDeepTransactionsVerify deepTransactionsVerify, CancellationToken cancellationToken)
        {
            try
            {
                block.Hash = await Miner.StartBlockMining(tailChain, block, MinerWaller, cancellationToken);
                BlockMined.Invoke(block);
            }
            catch (OperationCanceledException) { }
        }
    }
}
