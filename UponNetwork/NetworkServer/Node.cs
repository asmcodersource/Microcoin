using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using TcpNetwork;
using UponNetwork.NetworkSession;
using System.Net;

namespace UponNetwork.NetworkServer
{    
    public class Node
    {
        public NodeCrypto? NodeCrypto { get; protected set; }
        public NodeServer? NodeServer { get; protected set; }

        public void SendMessage(byte[] message)
        {
            NodeServer?.SendBroadcastMessage(message);
        }

        public async Task<bool> ConnectToNode(string addr, int port)
        {
            IPAddress listenerAddr = IPAddress.Parse(addr);
            IPEndPoint iPEndPoint = new IPEndPoint(listenerAddr, port);

            TcpConnection connection = new TcpConnection();
            connection.PacketInfoBuilder = new SessionPacketInfoBuilder();
            var success = await connection.ConnectToAsync(addr, port);
            if (!success)
                return false;
            success = await connection.VerifyConnection();
            if(!success)
                return false;

            NodeServer.SessionCreateHandler(this, connection);
            return true;
        }

        public void PrepareNodeCrypto()
        {
            var dir = System.AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.CreateDirectory(dir + "/keys");
            if( File.Exists(dir + "/keys/User.keys"))
                NodeCrypto = NodeCrypto.LoadKeysFromFile(dir + "/keys/User.keys");

            if( NodeCrypto == null )
            {
                NodeCrypto = new NodeCrypto();
                NodeCrypto.CreateKeys(2048);
                NodeCrypto.SaveKeysToFile(dir + "/keys/User.keys");
            }
        }

        public void PrepareNodeServer(int port = 1300)
        {
            if (NodeCrypto == null)
                throw new ApplicationException("NodeServer cant be initialized with out NodeCrypto");

            NodeServer = new NodeServer(this);
            var task = NodeServer.StartSpecificListener(IPAddress.Any, port);
            Task.WaitAny(task);
            if (task.Result == false)
                throw new ApplicationException("Something went wrong with node server initialize");
        }
    }
}
