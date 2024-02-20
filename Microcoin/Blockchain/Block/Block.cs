﻿using Microcoin.Blockchain.Transaction;
using System.Text.Json;
using Microcoin.Blockchain.Mining;

namespace Microcoin.Blockchain.Block
{
    internal class Block
    {
        public List<Transaction.Transaction> Transactions { get; set; } = new List<Transaction.Transaction>();
        public MiningBlockInfo MiningBlockInfo { get; set; } = new MiningBlockInfo();
        public string Hash { get; set; } = "";
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        public static Block? ParseBlockFromJson(string blockJson)
            => JsonSerializer.Deserialize<Block>(blockJson);
    }
}