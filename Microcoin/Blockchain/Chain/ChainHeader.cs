using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Chain
{
    [Serializable]
    public class ChainHeader
    {
        public Dictionary<string, double> WalletsCoins { get; protected set; }
        public Int32 BlocksCount { get; set; }

        public ChainHeader()
        { /* For JObject.Parse method */ }

        public ChainHeader(AbstractChain chain) 
        {
            //WalletsCoins = new Dictionary<string, double>(chain.WalletsCoins);
            BlocksCount = chain.GetBlocksList().Count();
        }
    }
}
