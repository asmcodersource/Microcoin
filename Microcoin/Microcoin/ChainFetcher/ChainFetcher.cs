using Chain;
using Block;

namespace Microcoin.Microcoin.ChainFetcher
{
    /// <summary>
    /// Part of blockchain realizations, responsible to loading long chains of blocks
    /// Chain defined by two blocks on start and end of loading part
    /// Loading allowen only by direct peer to peer connection
    /// </summary>
    public class ChainFetcher
    {
        public event Action<AbstractChain> ChainFetched;

        public void RequestChainFetch(Block.Block block)
        {

        }
    }
}
