using Microcoin.Blockchain.Chain;

namespace Microcoin.Blockchain.Mining
{
    public interface IRewardRule
    {
        public bool Verify(AbstractChain contextChain, Microcoin.Blockchain.Block.Block block);
        public double Calculate(AbstractChain contextChain, Microcoin.Blockchain.Block.Block block);
    }
}
