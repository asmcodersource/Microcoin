using Microcoin.Blockchain.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Mining
{
    public interface IRewardRule
    {
        public bool Verify(AbstractChain contextChain, Block.Block block);
        public double Calculate(AbstractChain contextChain, Block.Block block);
    }
}
