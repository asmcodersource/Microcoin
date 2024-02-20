﻿using Microcoin.Blockchain.Chain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Blockchain.Mining
{
    internal interface IRewardRule
    {
        public decimal CalculateReward(IChain contextChain, Block.Block block);
    }
}