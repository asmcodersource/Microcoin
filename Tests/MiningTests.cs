using Block;
using Transaction;
using Mining;

namespace Tests
{
    public class MiningTests
    {
        [Fact]
        public void OneBlockMiningTest()
        {
            // Create peer for transactions in chain 
            Peer peer = new Peer();
            peer.LoadOrCreateWalletKeys("NUL");
            peer.InitializeAcceptancePools();
           
            // Create first block with some transactions
            Block.Block block = new Block.Block();
            block.Hash = "none";
            block.MiningBlockInfo = new Mining.MiningBlockInfo();
            block.MiningBlockInfo.BlockId = 0;
            block.MiningBlockInfo.MinerReward = 1000;
            block.MiningBlockInfo.PreviousBlockHash = "none";
            block.Transactions.Add(peer.CreateTransaction(peer.WalletPublicKey, 0));


            Chain.Chain chain = new Chain.Chain();
            Miner miner = new Miner();
            miner.SetRules(new MiningRules(new ComplexityRule(), new RewardRule()));
            miner.StartBlockMining(chain, block, peer.WalletPublicKey, CancellationToken.None);
        }

        [Fact]
        public void TwoBlockMiningTest()
        {
            // Create peer for transactions in chain 
            Peer first_peer = new Peer();
            first_peer.LoadOrCreateWalletKeys("NUL");
            Peer second_peer = new Peer();
            second_peer.LoadOrCreateWalletKeys("NUL");

            // Create first block with some transactions
            Block.Block first_block = new Block.Block();
            first_block.Hash = "none";
            first_block.MiningBlockInfo = new Mining.MiningBlockInfo();
            first_block.MiningBlockInfo.BlockId = 0;
            first_block.MiningBlockInfo.MinerReward = 1000;
            first_block.MiningBlockInfo.PreviousBlockHash = "none";
            first_block.Transactions.Add(first_peer.CreateTransaction(first_peer.WalletPublicKey, 0));

            // add this first block to chain
            Chain.Chain chain = new Chain.Chain();
            Miner miner = new Miner();
            miner.SetRules(new MiningRules(new ComplexityRule(), new RewardRule()));
            var first_hash = miner.StartBlockMining(chain, first_block, first_peer.WalletPublicKey, CancellationToken.None).Result;
            first_block.Hash = first_hash;
            chain.AddTailBlock(first_block);

            // Create second block
            Block.Block second_block = new Block.Block();
            second_block.Transactions.Add(first_peer.CreateTransaction(second_peer.WalletPublicKey, 1.5));
            (miner as IMiner).LinkBlockToChain(chain, second_block);
            var second_hash = miner.StartBlockMining(chain, second_block, first_peer.WalletPublicKey, CancellationToken.None).Result;
            second_block.Hash = second_hash;

            // verify block is really correct by chain controller
            ChainController.ChainController chainController = new ChainController.ChainController(chain, miner);
            chainController.DefaultInitialize();
            Assert.True(chainController.AcceptBlock(second_block).Result);
        }
    }
}
