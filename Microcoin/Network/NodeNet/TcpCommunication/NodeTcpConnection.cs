﻿
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Serialization;
using Microcoin.Network.NodeNet.Communication;
using Newtonsoft.Json;

namespace Microcoin.Network.NodeNet.TcpCommunication
{
    public class NodeTcpConnection : INodeConnection
    {
        public bool IsListening { get; set; } = false;
        public TcpClient TcpClient { get; protected set; }
        public ITcpAddressProvider TcpAddressProvider { get; set; }
        public event Action<INodeConnection> MessageReceived;
        public event Action<INodeConnection> ConnectionClosed;
        protected Task? ListeningTask = null;
        protected JsonStreamParser.JsonStreamParser<Message.Message> jsonStreamParser = new JsonStreamParser.JsonStreamParser<Message.Message>();
        protected Queue<Message.Message> messagesQueue = new Queue<Message.Message>();


        public NodeTcpConnection()
        {
            TcpClient = new TcpClient();
        }

        public NodeTcpConnection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public bool Connect(string addr)
        {
            string ip = addr.Split(":")[0];
            string port = addr.Split(":")[1];
            try
            {
                jsonStreamParser = new JsonStreamParser.JsonStreamParser<Message.Message>();
                TcpClient.Connect(ip, Convert.ToInt32(port));
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public Message.Message? GetLastMessage()
        {
            return messagesQueue.Count != 0 ? messagesQueue.Dequeue() : null;
        }

        public List<Message.Message> GetMessageList()
        {
            var messageList = messagesQueue.ToList();
            messagesQueue.Clear();
            return messageList;
        }

        public void ListenMessages()
        {
            IsListening = true;
            Task.Run(() => MessageListener());
        }

        public void SendMessage(Message.Message message)
        {
            // Serialization can be performed in parallel, so lock is not needed here.
            var jsonMessage = JsonConvert.SerializeObject(message);
            var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
            var stream = TcpClient.GetStream();
            lock (this)
            {
                // Only one execution thread can write to a data stream at a time, otherwise it is impossible to interpret the data correctly.
                stream.Write(segment);
            }
        }

        public async Task SendRawData(byte[] data, CancellationToken cancellationToken)
        {
            await TcpClient.GetStream().WriteAsync(data, cancellationToken);
        }

        public async Task<byte[]> ReceiveRawData(CancellationToken cancellationToken)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024*16]);
            try
            {
                var size = await TcpClient.GetStream().ReadAsync(buffer, cancellationToken);
                return buffer.Array.Take(size).ToArray();
            }
            catch (Exception ex)
            {
                throw new WebSocketException("Socket closed");
            }
        }

        public void CloseConnection()
        {
            if (TcpClient == null)
                throw new Exception("Socket is not connected");
            if (IsListening)
                ConnectionClosed?.Invoke(this);
            IsListening = false;

        }

        protected async Task MessageListener()
        {
            // TODO: verify received part is correct json document, receive until correct json will be received
            var inputStream = TcpClient.GetStream();
            try
            {
                while (IsListening)
                {
                    var parsedObject = await jsonStreamParser.ParseJsonObjects(inputStream, CancellationToken.None);
                    foreach (var entry in parsedObject)
                        if (entry is Message.Message message)
                            AddMessageToQueue(message);
                }
            }
            catch (Exception ex)
            {
            }
            CloseConnection();
        }

        protected void AddMessageToQueue(Message.Message message)
        {
            messagesQueue.Enqueue(message);
            MessageReceived?.Invoke(this);
        }

        public string GetConnectionAddress()
        {
            if (TcpClient == null)
                throw new Exception("Connections is not initialized");
            if (TcpClient.Connected == false)
                throw new Exception("Connection is not active");

            string clientAddress = ((IPEndPoint)TcpClient.Client.LocalEndPoint).Address.MapToIPv4().ToString();
            int clientPort = TcpAddressProvider.GetNodeTcpPort();
            return clientAddress + ":" + clientPort;
        }
    }
}
