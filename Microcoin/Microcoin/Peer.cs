using Microcoin.Blockchain.Block;
using Microcoin.Blockchain.BlocksPool;
using Microcoin.Blockchain.ChainController;
using Microcoin.Blockchain.Mining;
using Microcoin.Blockchain.Transaction;
using Microcoin.Blockchain.TransactionsPool;

namespace Microcoin
{
    public class Peer
    {
        public BlocksPool BlocksPool { get; protected set; } = new BlocksPool();
        public ChainController ChainController { get; protected set; }
        public TransactionsPool TransactionsPool { get; protected set; } = new TransactionsPool();
        public PeerNetworking PeerNetworking { get; protected set; }
        public PeerWalletKeys PeerWalletKeys { get; protected set; }
        public PeerMining PeerMining { get; protected set; }


        public string WalletPublicKey { get { return PeerWalletKeys.TransactionSigner.SignOptions.PublicKey; } }

        public Transaction CreateTransaction(string receiverPublicKey, double coinsCount)
        {
            Transaction transaction = new Transaction();
            transaction.ReceiverPublicKey = receiverPublicKey;
            transaction.TransferAmount = coinsCount;
            PeerWalletKeys.SignTransaction(transaction);
            return transaction;
        }

        public Transaction SendCoins(string receiverPublicKey, double coinsCount)
        {
            if (PeerNetworking == null || PeerWalletKeys == null)
                throw new NullReferenceException("Peer is not initialized");

            var transaction = CreateTransaction(receiverPublicKey, coinsCount);
            TransactionsPool.HandleTransaction(transaction).Wait();
            return transaction;
        }

        public void InitializeAcceptancePools()
        {
            TransactionsPool.InitializeHandlerPipeline();
            BlocksPool.InitializeHandlerPipeline(TransactionsPool.HandlePipeline);
        }

        public void InitializeMining(bool miningEnable = true)
        {
            var complexityRule = new ComplexityRule();
            var rewardRule = new RewardRule();
            var miningRules = new MiningRules(complexityRule, rewardRule);
            Miner miner = new Miner();
            miner.SetRules(miningRules);
            PeerMining = new PeerMining();
            PeerMining.InizializeMiner(miner, WalletPublicKey, TransactionsPool);
            PeerMining.BlockMined += BlockMinedHandler;
            if (miningEnable)
                PeerMining.StartMining();

            TransactionsPool.OnTransactionReceived += (transaction)
                => PeerMining.TryStartMineBlock(ChainController.ChainTail, new DeepTransactionsVerify());
        }

        public void InitializeNetworking()
        {
            PeerNetworking = new PeerNetworking();
            PeerNetworking.CreateDefaultNode();
            PeerNetworking.CreateDefaultRouting();
            PeerNetworking.PostInitialize();
            PeerNetworking.TransactionReceived += (transaction) => TransactionsPool.HandleTransaction(transaction);
            PeerNetworking.BlockReceived += (block) => BlocksPool.HandleBlock(block);
        }

        public void LoadOrCreateWalletKeys(string filePath = "wallet.keys")
        {
            PeerWalletKeys = new PeerWalletKeys();
            if (File.Exists(filePath))
            {
                PeerWalletKeys.LoadKeys(filePath);
            }
            else
            {
                PeerWalletKeys.CreateKeys();
                PeerWalletKeys.SaveKeys(filePath);
            }
        }

        protected void BlockMinedHandler(Block block)
        {
            BlocksPool.HandleBlock(block).Wait();
            PeerNetworking.SendBlockToNetwork(block);
            PeerMining.StopMining();
            PeerMining.TryStartMineBlock(ChainController.ChainTail, new DeepTransactionsVerify());
        }
    }
}
