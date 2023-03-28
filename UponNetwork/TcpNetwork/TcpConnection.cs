using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using TcpNetwork;

namespace TcpNetwork
{
    public class TcpConnection: ITcpConnection
    {
        public ITcpPacketInfoBuilder PacketInfoBuilder { get; set; }
        public Socket Socket { get; set; }
        public ITcpServer Server { get; }
        protected byte[] CurrentReceiveBuffer { get; }
        protected CancellationTokenSource CancellationCurrentPacketReceive { get; set; }
        public bool IsVerified { get; set; }

        public TcpConnection()
        {
            CancellationCurrentPacketReceive = new CancellationTokenSource();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IsVerified = false;
        }

        ~TcpConnection()
        {
            DropConnection();
        }

        public override int GetHashCode()
        {
            return Socket.GetHashCode();
        }


        // Connection/Disconnection methods
        public void CreateConnectionBySocket(Socket socket)
        {
            this.Socket = socket;
        }

        public async Task<bool> ConnectToAsync(String ipAddr, int port)
        {
            // Prepare connecting params
            var addr = IPAddress.Parse(ipAddr);
            IPEndPoint ipEndPoint = new IPEndPoint(addr, port);

            // Try to connect with timeout
            try
            {
                var connectionTask = Socket.ConnectAsync(ipEndPoint);
                var timeoutTask = Task.Delay(1000);
                Task.WaitAny(new Task[] { connectionTask, timeoutTask });
            }
            catch
            {
                // In case of any error with connection
                // ignore them, just check socket connection
            }

            if (!Socket.Connected)
            {
                Socket.CancelConnectAsync(null);
                Socket.Close();
                return false;
            }

            this.SocketConnected?.Invoke(this, null);
            return true;
        }

        public void DropConnection()
        {
            Socket.Close();
            this.SocketDisconnected?.Invoke(this, this);
        }

        public async Task<bool> VerifyConnection()
        {
            IsVerified = false;
            string echoRequest = "TcpConnection echo request";
            string echoResponse = "TcpConnection echo response";
            byte[] receiveBuffer = new byte[echoResponse.Length];

            // Setup timeouts for echo, to prevent infinite waiting 
            Socket.SendTimeout = 500;
            Socket.ReceiveTimeout = 500;
            try
            {
                // Send echo request, and retrive response
                Socket.Send(Encoding.UTF8.GetBytes(echoRequest), SocketFlags.None);
                Socket.Receive(receiveBuffer, SocketFlags.None);
            }
            catch
            {
                // In case of any error, in communication
                return false;
            }
            Socket.SendTimeout = 0;
            Socket.ReceiveTimeout = 0;

            // Ensure response is equal to correct response 
            string response = Encoding.UTF8.GetString(receiveBuffer, 0, echoResponse.Length);
            if (response != echoResponse)
                return false;

            IsVerified = true;
            return true;
        }


        public async Task<bool> AcceptVerifyRequest()
        {
            IsVerified = false;
            string echoRequest = "TcpConnection echo request";
            string echoResponse = "TcpConnection echo response";
            byte[] receiveBuffer = new byte[echoRequest.Length];

            // Setup timeouts for echo, to prevent infinite waiting 
            Socket.SendTimeout = 500;
            Socket.ReceiveTimeout = 500;
            try
            {
                // Send echo request, and retrive response
                Socket.Receive(receiveBuffer, SocketFlags.None);
                Socket.Send(Encoding.UTF8.GetBytes(echoResponse), SocketFlags.None);
            }
            catch
            {
                // In case of any error, in communication
                return false;
            }
            Socket.SendTimeout = 0;
            Socket.ReceiveTimeout = 0;

            // Ensure response is equal to correct response 
            string response = Encoding.UTF8.GetString(receiveBuffer, 0, echoRequest.Length);
            if (response != echoRequest)
                return false;

            IsVerified = true;
            return true;
        }

        // Send/Receive methods
        public virtual void SendDataPacket(byte[] data, ITcpPacketInfo tcpPacketInfo = null)
        {
            if( tcpPacketInfo == null )
                tcpPacketInfo = PacketInfoBuilder.CreatePacketInfo();
            tcpPacketInfo.PacketSize = data.Length;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, tcpPacketInfo);

            Socket.Send(ms.ToArray());
            Socket.Send(data);
        }

        public async virtual Task<ReceivedPacket> ReceiveDataPacket()
        {
            byte[] packetInfo = new byte[PacketInfoBuilder.PacketInfoSize];

            // Receive packet info
            int receivedBytes = 0;
            while (receivedBytes < PacketInfoBuilder.PacketInfoSize) {
                var count = await Socket.ReceiveAsync(new ArraySegment<byte>(packetInfo), SocketFlags.None, CancellationCurrentPacketReceive.Token);
                receivedBytes = receivedBytes + count;
            }

            // Deserialize bytes to object
            var packetInfoObject = PacketInfoBuilder.DeserializePacketInfo(packetInfo);

            // Receive packet data
            receivedBytes = 0;
            byte[] packetData = new byte[packetInfoObject.PacketSize];
            while (receivedBytes < packetInfoObject.PacketSize)
            {
                var count = await Socket.ReceiveAsync(new ArraySegment<byte>(packetData), SocketFlags.None, CancellationCurrentPacketReceive.Token);
                receivedBytes = receivedBytes + count;
            }
            ReceivedPacket receivedPacket = new ReceivedPacket();
            receivedPacket.Info = packetInfoObject;
            receivedPacket.Data = packetData;
            return receivedPacket;
        }

        public void CancelCurrentPacketReceive()
        {
            CancellationCurrentPacketReceive.Cancel();
            CancellationCurrentPacketReceive = new CancellationTokenSource();
        }

        // Event methods
        public event EventHandler<object> DataReceived;
        public event EventHandler<ITcpConnection> SocketDisconnected;
        public event EventHandler<ITcpConnection> SocketConnected;
    }
}
