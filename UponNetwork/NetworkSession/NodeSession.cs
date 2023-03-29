using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpNetwork;
using UponNetwork.NetworkServer;

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
                    NewPacketReceivedHandler(receivedPacket);
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

        protected void NewPacketReceivedHandler(ReceivedPacket packet) 
        {
            // Just print out for example
            Console.WriteLine(Encoding.UTF8.GetString(packet?.Data));
        }
    }
}
