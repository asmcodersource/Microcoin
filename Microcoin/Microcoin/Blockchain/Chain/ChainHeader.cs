namespace Chain
{
    [Serializable]
    public class ChainHeader
    {
        public Dictionary<string, double> WalletsCoins { get; protected set; }
        public int BlocksCount { get; set; }

        public ChainHeader()
        { /* For JObject.Parse method */ }

        public ChainHeader(AbstractChain chain)
        {
            //WalletsCoins = new Dictionary<string, double>(chain.WalletsCoins);
            BlocksCount = chain.GetBlocksList().Count();
        }
    }
}
