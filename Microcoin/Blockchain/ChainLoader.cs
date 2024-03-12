using Microcoin.Blockchain.Block;
using Microcoin.Blockchain.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain
{
    /// <summary>
    /// Part of blockchain realizations, responsible to loading long chains of blocks
    /// Chain defined by two blocks on start and end of loading part
    /// Loading allowen only by direct peer to peer connection
    /// </summary>
    public class ChainLoader
    {
        public event Action<AbstractChain> ChainFetched;

        public void RequestChainFetch(Block.Block block)
        {

        }
    }
}
