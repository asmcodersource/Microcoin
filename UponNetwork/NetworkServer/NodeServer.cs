using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


using TcpNetwork;
using UponNetwork.NetworkSession;
using static System.Collections.Specialized.BitVector32;


namespace UponNetwork.NetworkServer
{
    public class NodeServer
    {
        public Node Node { get; protected set; }
        public HashSet<byte[]> ReceivedPacketsSet { get; protected set; }
        public Dictionary<IPEndPoint, ITcpServer> InterfaceListeners { get; protected set; }
        public Dictionary<ITcpConnection, NodeSession> NodeSessions { get; protected set; }

        public NodeServer(Node node) {
            this.Node = node;
            ReceivedPacketsSet = new HashSet<byte[]>();
            InterfaceListeners = new Dictionary<IPEndPoint, ITcpServer>();
            NodeSessions = new Dictionary<ITcpConnection, NodeSession>();
        }

        public async Task<bool> StartSpecificListener(string addr, int port) {
            IPAddress listenerAddr = IPAddress.Parse(addr);
            return await StartSpecificListener(listenerAddr, port);
        }

        public async Task<bool> StartSpecificListener(IPAddress listenerAddr, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(listenerAddr, port);
            if (InterfaceListeners.ContainsKey(iPEndPoint))
                return false;

            ITcpServer server = new TcpServer();
            server.ConnectionBuilder = new TcpConnectionBuilder();
            server.PacketInfoBuilder = new SessionPacketInfoBuilder();
            server.NewConnectionAccepted += SessionCreateHandler;
            server.NewConnectionRejected += SessionDropHandler;
            InterfaceListeners.Add(iPEndPoint, server);
            server.SetupListening(listenerAddr.ToString(), port);
            server.StartListeningAsync();
            return true;
        }

        public void StopSpecificListener(string addr, int port)
        {
            IPAddress listenerAddr = IPAddress.Parse(addr);
            StopSpecificListener(listenerAddr, port);
        }

        public void StopSpecificListener(IPAddress listenerAddr, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(listenerAddr, port);
            if (!InterfaceListeners.ContainsKey(iPEndPoint))
                return;

            var listener = InterfaceListeners[iPEndPoint];
            listener.StopListening();
            InterfaceListeners.Remove(iPEndPoint);
        }

        public async void SessionCreateHandler(object? sender, ITcpConnection tcpConnection)
        {
            var session = new NodeSession(tcpConnection, this);
            NodeSessions.Add(tcpConnection, session);
            session.NodeServer = this;
            session.StartReceiveCycle();
        }

        public void SessionDropHandler(object? sender, ITcpConnection tcpConnection)
        {
            var session = NodeSessions[tcpConnection];
            session.StopReceiveCycle();
            session.NodeServer = null;
            NodeSessions.Remove(tcpConnection);
        }

        public void SendBroadcastMessage(NodeSession session, ReceivedPacket packet)
        {
            foreach( var nodeSession in NodeSessions )
            {
                var node = nodeSession.Value;
                if (node == session)
                    continue;
                node.SendMessage(packet.Data, (SessionPacketInfo)packet.Info);
            }
        }

        public void SendBroadcastMessage( byte[] message )
        {
            foreach (var nodeSession in NodeSessions)
            {
                var node = nodeSession.Value;
                node.SendMessage(message);
            }
        }
    }
}
