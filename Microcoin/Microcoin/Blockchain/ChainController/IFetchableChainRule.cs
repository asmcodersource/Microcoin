using Microcoin.Blockchain.Chain;

namespace Microcoin.Blockchain.ChainController
{
    /// <summary>
    /// Some blocks may come from processing longer chains than the current one. 
    /// Not every one of them should be downloaded. 
    /// It is better to download the one that broke away further in the number of blocks, because this means that it is supported by a larger number of miners ( PoW ), 
    /// besides, in the future, the implementation of synchronization blacklists may be needed, and so on.
    /// </summary>
    public interface IFetchableChainRule
    {
        public bool IsPossibleChainUpgrade(AbstractChain chain, Microcoin.Blockchain.Block.Block block);
    }
}
