using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;
using UponNetwork.NetworkServer;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace UponNetwork.NetworkSession
{
    public class NodeSession
    {
        public NodeServer NodeServer { get; set; }
        public ITcpConnection TcpConnection { get; set; }
        protected CancellationTokenSource ReceiveCycleCancle;


        public NodeSession(ITcpConnection tcpConnection, NodeServer server)
        {
            ReceiveCycleCancle = new CancellationTokenSource();
            this.TcpConnection = tcpConnection;
            this.NodeServer = server;
        }

        public override int GetHashCode()
        {
            return TcpConnection.GetHashCode();
        }

        public async Task StartReceiveCycle()
        {
            if (!TcpConnection.IsVerified)
                throw new Exception("Connection is not verified!");

            try
            {
              while(!ReceiveCycleCancle.IsCancellationRequested) {
                    var receivedPacket = await TcpConnection.ReceiveDataPacket();
                    PacketReceiveHandler(receivedPacket);
              }  
            } catch( SocketException exception)
            {
                // Something went wrong
                TcpConnection.DropConnection();
            }
        }

        public void StopReceiveCycle()
        {
            ReceiveCycleCancle.Cancel();
            TcpConnection.CancelCurrentPacketReceive();
        }

        public void SendMessage(byte[] message, SessionPacketInfo? info = null)
        {
            if( info == null)
            {
                info = new SessionPacketInfo();
                info.MessageSign = new byte[0];
                info.PacketSize = message.Length;
                info.MessageSenderPublicKey = NodeServer.Node.NodeCrypto.publicKeyXml;
                info.MessageSign = NodeServer.Node.NodeCrypto.SignMessage(message);
            }

            RememberPacket(info);
            TcpConnection.SendDataPacket(message, info);
        }

        protected void PacketReceiveHandler(ReceivedPacket packet) 
        {
            SessionPacketInfo packetInfo = (SessionPacketInfo)packet.Info;
            if (!RememberPacket(packetInfo))
                return;
            var success = NodeCrypto.VerifyMessageSign(packet.Data, packetInfo.MessageSign, packetInfo.MessageSenderPublicKey);
            if (success == false)
                return;

            Console.WriteLine(Encoding.UTF8.GetString(packet.Data));
        }

        // Return false if packets duplicated
        protected bool RememberPacket(SessionPacketInfo packetInfo)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, packetInfo);
            HashAlgorithm algorithm = SHA256.Create();
            byte[] packetHash = algorithm.ComputeHash(ms.ToArray());
            if (NodeServer.ReceivedPacketsSet.Contains(packetHash))
                return false;
            NodeServer.ReceivedPacketsSet.Add(packetHash);
            return true;
        }
    }
}
