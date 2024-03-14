using Microcoin.Blockchain.Block;
using Microcoin.Microcoin;
using Microcoin.Microcoin.Blockchain.BlocksPool;

namespace Tests
{
    public class BlocksPipelineTests
    {
        [Fact]
        public void BlocksPipeline_ValidBlocks_Test()
        {
            BlocksPool blocksPool = new BlocksPool();

        }

        [Fact]
        public void BlockPipeline_Hash_Test() 
        {
            // Same block have to has same hash
            List<Peer> peers = TransactionTheory.CreateTestPeers(10);
            var transactionsTheories = TransactionTheory.GetValidTransactionsTheories(peers, 10);
            var block = new Block();
            foreach (var transactionTheory in transactionsTheories)
                block.Transactions.Add(transactionTheory.Transaction);
            block.Hash = block.GetMiningBlockHash();
            var firstHash = block.Hash;
            block.Hash = block.GetMiningBlockHash();
            block.Hash = block.GetMiningBlockHash();
            block.Hash = block.GetMiningBlockHash();
            block.Hash = block.GetMiningBlockHash();
            var lastHash = block.Hash;
            Assert.Equal(firstHash, block.Hash);
        }
    }
}
