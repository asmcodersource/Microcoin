using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace TcpNetwork
{
    public interface ITcpConnection
    {
        // Common connection fields
        public ITcpPacketInfoBuilder PacketInfoBuilder { get; set; }
        public Socket Socket { get; set; }
        public ITcpServer Server { get; }
        public bool IsVerified { get; set; }


        // Must be implemented
        public abstract int GetHashCode();


        // Connection/Disconnection methods
        public abstract void CreateConnectionBySocket(Socket socket);
        public abstract Task<bool> ConnectToAsync(String ipAddr, int port);
        public abstract void DropConnection();
        public abstract Task<bool> VerifyConnection();
        public abstract Task<bool> AcceptVerifyRequest();



        // Send/Receive methods
        public abstract void SendDataPacket(byte[] data, ITcpPacketInfo tcpPacketInfo = null);
        public abstract ReceivedPacket ReceiveDataPacket();


        // Event methods
        public event EventHandler<object> DataReceived;
        public event EventHandler<ITcpConnection> SocketDisconnected;
        public event EventHandler<ITcpConnection> SocketConnected;
    }
}
