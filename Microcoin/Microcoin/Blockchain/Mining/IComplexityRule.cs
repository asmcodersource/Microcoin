using Microcoin.Blockchain.Chain;

namespace Microcoin.Blockchain.Mining
{
    public interface IComplexityRule
    {
        public bool Verify(AbstractChain contextChain, Microcoin.Blockchain.Block.Block block);
        public int Calculate(AbstractChain contextChain, Microcoin.Blockchain.Block.Block block);
    }
}
