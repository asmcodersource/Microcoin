using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using System.Net.Sockets;

namespace TcpNetwork
{
    public  class TcpServer: ITcpServer
    {
        // Common server fields
        public ITcpPacketInfoBuilder PacketInfoBuilder { get; set; }
        public ITcpConnectionBuilder ConnectionBuilder { get; set; }
        protected CancellationTokenSource ListenCancellationSource { get; }
        public TcpListener Listener { get; set; }
        public HashSet<ITcpConnection> Connections { get; }


        public TcpServer()
        {
            ListenCancellationSource = new CancellationTokenSource();
            Connections = new HashSet<ITcpConnection>();
        }


        // Connections manage methods
        public void AddConnection(ITcpConnection connection)
        {
            Connections.Add(connection);
            connection.SocketDisconnected += RemoveConnectionHandler;
        }


        public void RemoveConnectionHandler(object sender, ITcpConnection connection)
        {
            connection.SocketDisconnected -= RemoveConnectionHandler;
            Connections.Remove(connection);
            ConnectionDroped?.Invoke(this, connection);
        }


        // Listen methods
        public void SetupListening(String ipAddr, int port)
        {
            var addr = IPAddress.Parse(ipAddr);
            IPEndPoint iPEndPoint = new IPEndPoint(addr, port);
            Listener = new TcpListener(iPEndPoint);
        }

        public async Task StartListeningAsync()
        {
            Listener.Start();

            while (!ListenCancellationSource.IsCancellationRequested)
            {
                var incommingSoket = await Listener.AcceptSocketAsync(ListenCancellationSource.Token);
                var acceptanceTask = AcceptConnection(incommingSoket);
            }
        }

        protected async Task AcceptConnection(Socket incommingSocket)
        {
            ITcpConnection connection = ConnectionBuilder.CreateConnection();
            connection.PacketInfoBuilder = this.PacketInfoBuilder;
            connection.CreateConnectionBySocket(incommingSocket);
            bool success = await connection.AcceptVerifyRequest();
            if( success == false)
            {
                this.NewConnectionRejected?.Invoke(this, null);
                connection.DropConnection();
                return;
            }

            AddConnection(connection);
            this.NewConnectionAccepted?.Invoke(this, connection);
        }

        public void StopListening()
        {
            Listener.Stop();
            ListenCancellationSource.Cancel();
        }

        // Event methods
        public event EventHandler<ITcpConnection> NewConnectionAccepted;
        public event EventHandler<ITcpConnection> NewConnectionRejected;
        public event EventHandler<ITcpConnection> ConnectionDroped;
    }
}
