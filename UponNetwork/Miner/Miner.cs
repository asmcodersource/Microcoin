using Microcoin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Miner
{
    public class Miner
    {
        public CancellationTokenSource CancelMiningSource { get; protected set; }

        public async Task<ulong> MineBlock(Block blockToMine, int complexity)
        {
            Random random = new Random();
            CancelMiningSource = new CancellationTokenSource();
            ulong? resultMagikValue = null;

            await Task.Run(() => {
                while (CancelMiningSource.IsCancellationRequested == false)
                {
                    var magikValue = (ulong)random.NextInt64();
                    blockToMine.MagikValue = (ulong)magikValue;
                    var blockHash = blockToMine.BlockHash();
                    bool blockFail = false;
                    for (int i = 0; i < complexity; i++)
                    {
                        if ((blockHash[i / 8] & (i % 8)) != 0)
                        {
                            blockFail = true;
                            break;
                        }
                    }
                    if (blockFail == false )
                    {
                        resultMagikValue = magikValue;
                        return;
                    }
                }
            });

            if( resultMagikValue is not null )
                MiningComplete?.Invoke(this, resultMagikValue.Value);
            return resultMagikValue.Value;
        }

        public event EventHandler<ulong> MiningComplete;
    }
}
