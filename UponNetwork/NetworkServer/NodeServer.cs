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
        public Node Node { get; set; }
        Dictionary<IPEndPoint, ITcpServer> interfaceListeners = new  Dictionary<IPEndPoint, ITcpServer>();
        Dictionary<ITcpConnection, NodeSession> nodeSessions = new Dictionary<ITcpConnection, NodeSession>();

        public NodeServer(Node node) {
            this.Node = node;
        }

        public async Task<bool> StartSpecificListener(string addr, int port) {
            IPAddress listenerAddr = IPAddress.Parse(addr);
            return await StartSpecificListener(listenerAddr, port);
        }

        public async Task<bool> StartSpecificListener(IPAddress listenerAddr, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(listenerAddr, port);
            if (interfaceListeners.ContainsKey(iPEndPoint))
                return false;

            ITcpServer server = new TcpServer();
            server.ConnectionBuilder = new TcpConnectionBuilder();
            server.PacketInfoBuilder = new SessionPacketInfoBuilder();
            server.NewConnectionAccepted += SessionCreateHandler;
            server.NewConnectionRejected += SessionDropHandler;
            interfaceListeners.Add(iPEndPoint, server);
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
            if (!interfaceListeners.ContainsKey(iPEndPoint))
                return;

            var listener = interfaceListeners[iPEndPoint];
            listener.StopListening();
            interfaceListeners.Remove(iPEndPoint);
        }

        public void SessionCreateHandler(object? sender, ITcpConnection tcpConnection)
        {
            var session = new NodeSession(tcpConnection);
            nodeSessions.Add(tcpConnection, session);
            session.NodeServer = this;
            session.StartReceiveCycle();
        }

        public void SessionDropHandler(object? sender, ITcpConnection tcpConnection)
        {
            var session = nodeSessions[tcpConnection];
            session.StopReceiveCycle();
            session.NodeServer = null;
            nodeSessions.Remove(tcpConnection);
        }

        public void SendBroadcastMessage(NodeSession session, byte[] message)
        {
            foreach( var nodeSession in nodeSessions )
            {
                var node = nodeSession.Value;
                if (node == session)
                    continue;


            }
        }
    }
}
