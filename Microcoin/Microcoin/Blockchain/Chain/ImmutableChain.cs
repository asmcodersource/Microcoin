namespace Chain
{
    [Serializable]
    public class ImmutableChain : AbstractChain
    {

        public ImmutableChain(AbstractChain chain)
        {
            TransactionsSet = new HashSet<Transaction.Transaction>(chain.TransactionsSet);
            WalletsCoins = new Dictionary<string, double>(chain.WalletsCoins);
            PreviousChain = chain.PreviousChain;
            blocksList = new List<Block.Block>(chain.GetBlocksList());
            BlocksDictionary = new Dictionary<string, Block.Block>(chain.BlocksDictionary);
        }
    }
}
