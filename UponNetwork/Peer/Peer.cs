using Microcoin.UponNetwork.NetworkNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microcoin.UponNetwork.NetworkNode;

namespace Microcoin.Peer
{
    public class Peer
    {
        public MessageHandler? MessageHandler { get; protected set; }
        public Node? Node { get; protected set; }


        public async Task InitializeNode(Settings.Settings settings)
        {
            Node = new Node();
            Node.PrepareNodeCrypto(settings.PeerNetworkSettings.PeerKeysFileName);
            Node.PrepareNodeServer(settings.PeerNetworkSettings.ListeningPort);
            this.MessageHandler = new MessageHandler(this);

            Node.LoadPeersFromFile(settings.PeerNetworkSettings.PeerKeysFileName);
            await Node.CreateSessionsByPeersStorage();
        }
    }
}
