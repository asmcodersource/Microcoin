﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.ChainController
{
    /// <summary>
    /// Some blocks may come from processing longer chains than the current one. 
    /// Not every one of them should be downloaded. 
    /// It is better to download the one that broke away further in the number of blocks, because this means that it is supported by a larger number of miners ( PoW ), 
    /// besides, in the future, the implementation of synchronization blacklists may be needed, and so on.
    /// </summary>
    internal interface IFetchableChainRule
    {
        public bool IsPossibleChainUpgrade(Block.Block block);
    }
}
