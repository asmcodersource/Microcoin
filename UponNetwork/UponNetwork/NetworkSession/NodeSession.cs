using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microcoin.UponNetwork.NetworkNode;


namespace Microcoin.UponNetwork.NetworkSession
{
    public class NodeSession
    {
        public NodeServer NodeServer { get; set; }
        public ITcpConnection TcpConnection { get; set; }
        protected CancellationTokenSource ReceiveCycleCancle;


        public NodeSession(ITcpConnection tcpConnection, NodeServer server)
        {
            ReceiveCycleCancle = new CancellationTokenSource();
            TcpConnection = tcpConnection;
            NodeServer = server;
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
                while (!ReceiveCycleCancle.IsCancellationRequested)
                {
                    var receivedPacket = await TcpConnection.ReceiveDataPacket();
                    Console.WriteLine("Some message received!");
                    PacketReceiveHandler(receivedPacket);
                }
            }
            catch (SocketException exception)
            {
                // Something went wrong
                TcpConnection.DropConnection();
                Console.WriteLine("Some connection broken at receiving");
            }
        }

        public void StopReceiveCycle()
        {
            ReceiveCycleCancle.Cancel();
            TcpConnection.CancelCurrentPacketReceive();
        }

        public void SendMessage(byte[] message, SessionPacketInfo? info = null, bool isTechnical = false, int peersToPass = -1)
        {
            if (info == null)
            {
                info = new SessionPacketInfo();
                info.PeersToPass = -1;
                info.MessageSign = new byte[0];
                info.PacketSize = message.Length;
                info.MessageSenderPublicKey = NodeServer.Node.NodeCrypto.publicKeyXml;
                info.MessageSign = NodeServer.Node.NodeCrypto.SignMessage(message);
            }

            info.IsTehnicalPacket = isTechnical;
            info.PeersToPass = peersToPass;
            try
            {
                TcpConnection.SendDataPacket(message, info);
                RememberPacket(info);
            }
            catch (SocketException exception)
            {
                // Something went wrong
                TcpConnection.DropConnection();
                Console.WriteLine("Some connection broken at sending");
            }
        }

        protected void PacketReceiveHandler(ReceivedPacket packet)
        {
            SessionPacketInfo packetInfo = (SessionPacketInfo)packet.Info;
            if (packetInfo.PeersToPass != -1)
                packetInfo.PeersToPass -= 1;
            //if (!RememberPacket(packetInfo))
            //    return;
            var success = NodeCrypto.VerifyMessageSign(packet.Data, packetInfo.MessageSign, packetInfo.MessageSenderPublicKey);
            if (success == false)
                return;
            if (packetInfo.MessageSenderPublicKey == NodeServer.Node.NodeCrypto?.publicKeyXml)
                return;

            if (packetInfo.PeersToPass != 0)
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
