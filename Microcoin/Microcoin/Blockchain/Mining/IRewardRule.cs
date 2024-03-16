using Chain;

namespace Mining
{
    public interface IRewardRule
    {
        public bool Verify(AbstractChain contextChain, Block.Block block);
        public double Calculate(AbstractChain contextChain, Block.Block block);
    }
}
