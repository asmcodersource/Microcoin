﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Chain
{
    internal class ImmutableChain : IChain
    {
        public ImmutableChain? PreviousChain { get; }
        public List<Block.Block> BlocksList { get; }
        public Dictionary<string, Block.Block> BlocksDictionary { get; }

        public ImmutableChain(Chain chain)
        {
            throw new NotImplementedException();
        }
    }
}