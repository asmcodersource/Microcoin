using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using TcpNetwork;
using Microcoin.UponNetwork.NetworkSession;
using System.Net;
using static System.Collections.Specialized.BitVector32;

namespace Microcoin.UponNetwork.NetworkNode
{
    public class Node
    {
        public int ListeningPort { get; protected set; }
        public NodePeersStorage? NodePeersStorage { get; protected set; }
        public NodeCrypto? NodeCrypto { get; protected set; }
        public NodeServer? NodeServer { get; protected set; }
        public NodeDiscovery? NodeDiscovery { get; protected set; }

        public void SendMessage(byte[] message, bool isTechnical = false, int peersToPass = -1)
        {
            NodeServer?.SendBroadcastMessage(message, isTechnical, peersToPass);
        }

        public async Task<bool> ConnectToNode(string addr, int port)
        {
            IPAddress listenerAddr = IPAddress.Parse(addr);
            IPEndPoint iPEndPoint = new IPEndPoint(listenerAddr, port);

            TcpConnection connection = new TcpConnection();
            connection.PacketInfoBuilder = new SessionPacketInfoBuilder();
            var success = await connection.ConnectToAsync(addr, port);
            if (success == false)
                return false;
            success = await connection.VerifyConnection();
            if (success == false)
                return false;

            NodeServer.SessionCreateHandler(this, connection);
            NodePeersStorage.AddPeer(new Peer(addr, port));
            return true;
        }

        public void PrepareNodeCrypto(string fileName)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(dir + "/keys");
            string filePath = dir + "/keys/" + fileName;
            if (File.Exists(filePath))
                NodeCrypto = NodeCrypto.LoadKeysFromFile(filePath);

            if (NodeCrypto == null)
            {
                NodeCrypto = new NodeCrypto();
                NodeCrypto.CreateKeys(2048);
                NodeCrypto.SaveKeysToFile(filePath);
            }
        }

        public void PrepareNodeServer(int port = 1300)
        {
            ListeningPort = port;
            if (NodeCrypto == null)
                throw new ApplicationException("NodeServer cant be initialized with out NodeCrypto");

            NodeServer = new NodeServer(this);
            NodeServer.SessionReceivedMessage += (sender, packet) => NodeReceivedMessage?.Invoke(this, packet);
            NodeServer.SessionReceivedTechnicalMessage += (sender, packet) => NodeReceivedTechnicalMessage?.Invoke(this, packet);
            var task = NodeServer.StartSpecificListener(IPAddress.Any, port);

            Task.WaitAny(task);
            if (task.Result == false)
                throw new ApplicationException("Something went wrong with node server initialize");

            NodeDiscovery = new NodeDiscovery(this);
        }

        public void LoadPeersFromFile(string fileName)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(dir + "storage");
            if (!File.Exists(dir + "/storage/" + fileName))
                SavePeersToFile(fileName);
            NodePeersStorage = new NodePeersStorage();
            NodePeersStorage.LoadPeers(dir + "/storage/" + fileName);
        }

        public void SavePeersToFile(string fileName)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(dir + "storage");
            NodePeersStorage = new NodePeersStorage();
            NodePeersStorage.SavePeers(dir + "/storage/" + fileName);
        }


        public async Task CreateSessionsByPeersStorage()
        {
            var peers = NodePeersStorage.Peers.ToArray();
            foreach (var peer in peers)
                await ConnectToNode(peer.Address, peer.Port);

        }

        public event EventHandler<ReceivedPacket> NodeReceivedMessage;
        public event EventHandler<ReceivedPacket> NodeReceivedTechnicalMessage;
    }
}
