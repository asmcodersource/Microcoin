namespace Microcoin.Blockchain.Mining
{
    [Serializable]
    public class MiningBlockInfo
    {
        public int BlockId { get; set; } = -1;
        public long MinedValue { get; set; } = 0;
        public string PreviousBlockHash { get; set; } = "";
        public string MinerPublicKey { get; set; } = "";
        public double MinerReward { get; set; } = 0.0;
        public int ChainComplexity { get; set; } = 0;
        public int Complexity { get; set; } = 0;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}
