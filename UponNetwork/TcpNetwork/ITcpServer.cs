using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using System.Net.Sockets;

namespace TcpNetwork
{
    public interface ITcpServer
    {
        // Common server fields
        public ITcpPacketInfoBuilder PacketInfoBuilder { get; set; }
        public ITcpConnectionBuilder ConnectionBuilder { get; set; }
        public TcpListener Listener { get; set; }
        public HashSet<ITcpConnection> Connections { get; }

        

        // Connections manage methods
        public abstract void AddConnection( ITcpConnection connection );
        public abstract void RemoveConnectionHandler(object sender, ITcpConnection e); 


        // Listen methods
        public abstract void SetupListening(String ipAddr, int port);
        public abstract Task StartListeningAsync();
        public abstract void StopListening();


        // Event methods
        public event EventHandler<ITcpConnection> NewConnectionAccepted;
        public event EventHandler<ITcpConnection> NewConnectionRejected;
        public event EventHandler<ITcpConnection> ConnectionDroped;
    }
}
