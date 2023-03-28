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


        public NodeSession(ITcpConnection tcpConnection)
        {
            ReceiveCycleCancle = new CancellationTokenSource();
            this.TcpConnection = tcpConnection;
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
                
              }      
            } catch( SocketException exception)
            {
                // Something went wrong
                TcpConnection.DropConnection();
            }
        }
    }
}
