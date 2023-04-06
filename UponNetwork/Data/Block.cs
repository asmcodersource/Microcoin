using Microcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Data
{
    [Serializable]
    public class Block : ISignable
    {
        public List<Transaction> Transactions { get; set; }
        public DateTime CreationTime { get; set; }
        public decimal MiningReward { get; set; }
        public byte[] MagikValue { get; set; }
        public string MinerWallet { get; set; }
        public string Signature { get; set; }
    }
}
