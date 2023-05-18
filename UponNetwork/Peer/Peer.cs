using Microcoin.UponNetwork.NetworkNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcoin.Crypto;
using Microcoin.Data;
using Microcoin.Settings;
using Microcoin.Mining;
using System.Transactions;

namespace Microcoin.Peer
{
    public class Peer
    {
        public MessageHandler? MessageHandler { get; protected set; }
        public Node? Node { get; protected set; }
        public ICryptoKeys? CryptoKeys { get; protected set; }
        public Blockchain Blockchain { get; protected set; }
        public TransactionsPool TransactionsPool { get; protected set; }
        public Miner Miner { get; protected set; }
        public Task<ulong>? MiningTask { get; protected set; }


        public async Task InitializePeer(Settings.Settings settings)
        {
            InitializeMiner(settings);
            InitializeCryptoKeys(settings);
            await InitializeBlockchain(settings);
            await InitializeNode(settings);
        }

        public void SendCoins(decimal coinsCount, string receiverWallet)
        {
            if (Node == null)
                throw new ApplicationException("Peer is not initialized");

            Message message = new Message();
            message.MessageType = MessageType.CreateNewTransaction;
            message.SendingTime = DateTime.UtcNow;
            message.ReceiverPublicKey = "";
            message.SenderPublicKey = ((CryptoKeys)CryptoKeys).PublicKeyXml;

            Microcoin.Data.Transaction transaction = new Microcoin.Data.Transaction();
            transaction.CoinsToSend = coinsCount;
            transaction.ReceiverWallet = receiverWallet;
            transaction.SenderWallet = ((CryptoKeys)CryptoKeys).PublicKeyXml;
            transaction.CreationTime = DateTime.UtcNow;

            message.MessageObject = transaction;
            Signer signer = new Signer();
            signer.SetKeys(CryptoKeys);
            signer.Sign(transaction);
            signer.Sign(message);

            Node.SendMessage(message.Serialize());
        }

        public void SendMinedBlock(Block block)
        {
            if (Node == null)
                throw new ApplicationException("Peer is not initialized");

            Message message = new Message();
            message.MessageType = MessageType.NewBlockMined;
            message.SendingTime = DateTime.UtcNow;
            message.ReceiverPublicKey = "";
            message.SenderPublicKey = ((CryptoKeys)CryptoKeys).PublicKeyXml;

            message.MessageObject = block;
            Signer signer = new Signer();
            signer.SetKeys(CryptoKeys);
            signer.Sign(message);

            Node.SendMessage(message.Serialize());
        }

        protected async Task InitializeNode(Settings.Settings settings)
        {
            Node = new Node();
            this.MessageHandler = new MessageHandler(this);
            this.Node.NodeReceivedMessage += this.MessageHandler.HandleMessage;
            Node.PrepareNodeCrypto(settings.PeerNetworkSettings.PeerKeysFileName);
            Node.PrepareNodeServer(settings.PeerNetworkSettings.ListeningPort);
            Node.LoadPeersFromFile(settings.PeerNetworkSettings.PeerKeysFileName);
            //Node.NodeDiscovery.StartBeacon();
            await Node.CreateSessionsByPeersStorage();
        } 

        protected void InitializeCryptoKeys(Settings.Settings settings)
        {
            if (settings.BlockchainSettings.WalletKeysFile == null)
                throw new ApplicationException("Blockchain settings has wrong parameter of WalletKeysFile");

            var dir = AppDomain.CurrentDomain.BaseDirectory;
            string keysFilePath = Path.Combine(dir, settings.BlockchainSettings.WalletKeysFile);
            if (File.Exists(keysFilePath))
            {
                CryptoKeys = new CryptoKeys();
                CryptoKeys.LoadKeys(keysFilePath);
            } else
            {
                CryptoKeys = new CryptoKeys();
                CryptoKeys.CreateKeys();
                CryptoKeys.SaveKeys(keysFilePath);
            }
        }

        protected async Task InitializeBlockchain(Settings.Settings settings)
        {
            var begginerWaller = "<RSAKeyValue><Modulus>zF6cNhyqjHsf88OLnPSe3XyMpCzBHVBTcalgWMI4MnHuWbZ2XaCSYoc3m/3k4c/s+mtYY7B0AEr/7yviob+mcoyP6S1M/xzPw4NQAmB1F+CxhpKTbSkeh9y+IFaplVLLYS9zEceEZMY0ygiGYlqCxsAKRhWff888fo/nyZdZMf4EU1/sZUOtSPjUbIngDbj9NiFg6FLsd9MfiwU3VplP4Xk7xGsbZZvKVrDYlv8chDSflEqn5Dj64vfGkIanchHrW6DXDa5GK+TceUSIoi5qaRD5qlUDcrliDgzkVpQoGsVqtIvU1QULcOVKrVZKIg93Cs9Uu1OXvq8xgK3Fs/koAQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var initialBlock = Blockchain.CreateInitialBlock(begginerWaller);
            this.Blockchain = new Blockchain(this);
            this.Blockchain.Blocks.Add(initialBlock);
            this.TransactionsPool = new TransactionsPool(this.Blockchain);
            this.TransactionsPool.NewTransactionConfirmed += TransactionsConfirmHandler;
        }

        protected void InitializeMiner(Settings.Settings settings)
        {
            Miner = new Miner();
            Miner.MiningComplete += MiningCompleteHandler;
        }

        protected async void TransactionsConfirmHandler(object sender, EventArgs eventArgs)
        {
            lock (this)
            {
                if (Miner.IsMining == true)
                    return;

                Console.WriteLine("Start mining");
                Block block = TransactionsPool.ClaimNextBlock();
                Miner.StartBlockMining(block, Blockchain.Blocks[^1].CreationTime);
            }
        }

        protected void MiningCompleteHandler(object sender, ulong magikValue)
        {
            lock (this)
            {
                var block = Miner.LastMiningBlock;
                block.MagikValue = magikValue;
                byte[] hash = block.BlockHash();

                int i = 0; 
                for (i = 0; i < 50; i++)
                    if ((hash[i / 8] & (i % 8)) != 0)
                        break;
                
                bool success = Blockchain.AppendBlockToChain(block);
                if (success)
                {
                    SendMinedBlock(block);
                    Console.WriteLine("Mining complete!");
                    Console.WriteLine("Block creation time = " + block.CreationTime.ToShortTimeString());
                    Console.WriteLine("Complexity = " + i);
                }
            }
        }
    }
}
