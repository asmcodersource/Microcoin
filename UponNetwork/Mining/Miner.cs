using Microcoin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Mining
{
    public class Miner
    {
        public bool? IsMining { get; set; } = false;
        protected List<Task> MinerTasks { get; set; } = new List<Task>(); 
        public CancellationTokenSource CancelMiningSource { get; set; }
        protected DateTime MiningStartTime { get; set; }
        public Block LastMiningBlock { get; protected set; }

        public void StartBlockMining( Block block, DateTime startMiningTime )
        {
            LastMiningBlock = block;
            IsMining = new bool();
            IsMining = true;
            MinerTasks.Clear();
            CancelMiningSource = new CancellationTokenSource();
            MiningStartTime = startMiningTime;
            for ( int i = 0; i < Environment.ProcessorCount; i++ )
            {
                var task = MineBlock(block.Clone(), CancelMiningSource.Token);
                MinerTasks.Add(task);
            }
        }

        public async Task MineBlock( Block targetBlock, CancellationToken cancellationToken  )
        {
            Random random = new Random();
            while (cancellationToken.IsCancellationRequested == false )
            {
               int complexity = CalculateComplexity(DateTime.UtcNow.Subtract(MiningStartTime).TotalMilliseconds);
               for ( int i = 0; i < 4096; i++)
               {    
                    var magikValue = (ulong)random.NextInt64();
                    targetBlock.MagikValue = (ulong)magikValue;
                    var blockHash = targetBlock.BlockHash();
                    if( VerifyHashComplexity(blockHash, complexity) == true)
                    {
                        lock (this)
                        {
                            if (IsMining == false)
                                return;
                            CancelMiningSource.Cancel();
                            MiningComplete?.Invoke(this, magikValue);
                            IsMining = false;
                            return;
                        }
                    }
                }
                await Task.Yield();
            }
            IsMining = false;
        }

        public static bool VerifyHashComplexity(byte[] hash, int complexity)
        {
            for (int i = 0; i < complexity; i++)
                if ((hash[i / 8] & (i % 8)) != 0)
                    return false;
            return true;
        }

        public int CalculateComplexity(double passedMilliseconds )
        {
            // https://www.desmos.com/calculator/6ivsjgpcul
            var x = passedMilliseconds / 1000;
            x = x <= 600 ? 600.000000001 : x; 
            var c = (280.0 / (0.05*(x - 600))) + 25.0;
            c = c > 63 ? 63 : c;
            return (int)c;
        }

        public event EventHandler<ulong> MiningComplete;
    }
}
