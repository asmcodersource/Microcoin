using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Chain
{
    /// <summary>
    /// To attach a block, you need to determine whether this block can be a continuation of the chain. 
    /// This interface defines the class contract that implements this rule
    /// </summary>
    public interface INextBlockRule
    {
        public bool IsBlockNextToChain(Block.Block block, AbstractChain chain);
    }
}
