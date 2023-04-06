using Microcoin.UponNetwork.NetworkNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcoin.Crypto;

namespace Microcoin.Peer
{
    public class Peer
    {
        public MessageHandler? MessageHandler { get; protected set; }
        public Node? Node { get; protected set; }
        public ICryptoKeys? CryptoKeys { get; protected set; }


        public async Task InitializePeer(Settings.Settings settings)
        {
            await InitializeNode(settings);
            InitializeCryptoKeys(settings);
        }

        protected async Task InitializeNode(Settings.Settings settings)
        {
            Node = new Node();
            this.MessageHandler = new MessageHandler(this);
            this.Node.NodeReceivedMessage += this.MessageHandler.HandleMessage;
            Node.PrepareNodeCrypto(settings.PeerNetworkSettings.PeerKeysFileName);
            Node.PrepareNodeServer(settings.PeerNetworkSettings.ListeningPort);
            Node.LoadPeersFromFile(settings.PeerNetworkSettings.PeerKeysFileName);
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
    }
}
