using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;
using UponNetwork.NetworkNode;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;


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

        public void SendMessage(byte[] message, SessionPacketInfo? info = null, bool isTechnical = false)
        {
            if( info == null)
            {
                info = new SessionPacketInfo();
                info.MessageSign = new byte[0];
                info.PacketSize = message.Length;
                info.MessageSenderPublicKey = NodeServer.Node.NodeCrypto.publicKeyXml;
                info.MessageSign = NodeServer.Node.NodeCrypto.SignMessage(message);
            }

            info.IsTehnicalPacket = isTechnical;
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
            if (packetInfo.MessageSenderPublicKey == NodeServer.Node.NodeCrypto?.publicKeyXml)
                return;

            NodeServer.SendBroadcastMessage(this, packet);
            if (packetInfo.IsTehnicalPacket)
                TechnicalMessageReceived.Invoke(this, packet);
            else
                MessageReceived.Invoke(this, packet);
        }

        // Return false if packets duplicated
        protected bool RememberPacket(SessionPacketInfo packetInfo)
        {
            int packetHash = packetInfo.GetHashCode();
            if (NodeServer.ReceivedPacketsSet.Contains(packetHash))
                return false;
            NodeServer.ReceivedPacketsSet.Add(packetHash);
            return true;
        }

        public event EventHandler<ReceivedPacket> MessageReceived;
        public event EventHandler<ReceivedPacket> TechnicalMessageReceived;
    }
}
