using Chain;

namespace Mining
{
    public interface IComplexityRule
    {
        public bool Verify(AbstractChain contextChain, Block.Block block);
        public int Calculate(AbstractChain contextChain, Block.Block block);
    }
}
