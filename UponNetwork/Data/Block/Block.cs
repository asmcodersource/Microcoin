using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Data.Block
{
    [Serializable]
    public class Block
    {
        public decimal MiningReward { get; set; }
        public byte[] MagikValue { get; set; }
        public string MinerWallet { get; set; }
        public string BlockSign { get; set; }
    }
}
