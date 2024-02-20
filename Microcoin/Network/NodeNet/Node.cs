﻿using Microcoin.Network.NodeNet.TcpCommunication;
using Microcoin.Network.NodeNet.ReceiveMiddleware;
using Microcoin.Network.NodeNet.Communication;
using Microcoin.Network.NodeNet.NodeActions;
using Microcoin.Network.NodeNet.Message;
using Microcoin.RSAEncryptions;

namespace Microcoin.Network.NodeNet
{
    internal class Node : ITcpAddressProvider
    {
        public IMessageSigner? MessageSigner { get; protected set; } = null;
        public IMessageValidator? MessageValidator { get; protected set; } = null;
        public INodeListener? ConnectionsListener { get; protected set; } = null;
        public INodeConnections? Connections { get; protected set; } = null;
        public ISenderSignOptions? SignOptions { get; protected set; } = null;
        public IReceiveMiddleware ReceiveMiddlewareHead { get; protected set; }
        public NetworkExplorer.NetworkExplorer NetworkExplorer { get; protected set; }

        public event Action<MessageContext> MessageReceived;

        public static Node CreateRSAHttpNode(SenderSignOptions options, TcpListenerOptions listenerOptions)
        {

            IMessageSigner messageSigner = new MessageSigner();
            IMessageValidator messageValidator = new MessageValidator();
            messageSigner.SetSignOptions(options);

            Node node = new Node();
            node.SignOptions = options;
            node.MessageValidator = messageValidator;
            node.MessageSigner = messageSigner;
            node.Connections = new TcpCommunication.TcpCommunication();
            node.NetworkExplorer = new NetworkExplorer.NetworkExplorer(node);

            // Middleware pipeline
            var signMiddleware = new SignVerificationMiddleware(node, messageValidator);
            var cacheMiddleware = new MessageCacheMiddleware();
            var floodProtectorMiddleware = new FloodProtectorMiddleware();
            signMiddleware.SetNext(floodProtectorMiddleware);
            floodProtectorMiddleware.SetNext(cacheMiddleware);
            cacheMiddleware.SetNext(node.NetworkExplorer.Middleware);
            node.ReceiveMiddlewareHead = signMiddleware;
            // TODO: add another middlewares in pipeline

            var listener = new NodeTcpListener();
            listener.Options = listenerOptions;
            node.ConnectionsListener = listener;
            node.ConnectionsListener.ConnectionOpened += node.NewConnectionHandler;
            node.ConnectionsListener.StartListening();

            return node;
        }

        public void SendMessage(string messageContent, string receiver = null, bool isTechnical = false)
        {
            if (MessageSigner == null || SignOptions == null || Connections == null)
                throw new Exception("Node is not initialized!");

            if (receiver == null)
                receiver = string.Empty;
            var connections = Connections.Connections();
            var messageInfo = new MessageInfo(SignOptions.PublicKey, receiver, isTechnical);
            var message = new Message.Message(messageInfo, messageContent);
            MessageSigner.Sign(message);
            foreach (var connection in connections)
                connection.SendMessage(message);
        }

        public bool Connect(string url)
        {
            NodeTcpConnection connection = new NodeTcpConnection();
            connection.TcpAddressProvider = this;
            bool result = connection.Connect(url);
            if (result == false)
                return false;
            var pingTask = PingPong.Ping(connection);
            pingTask.Wait();
            if (pingTask.Result)
            {
                NewConnectionHandler(connection);
                return true;
            }
            return false;
        }

        public void Close()
        {
            if (ConnectionsListener == null || Connections == null)
                throw new Exception("Node is not initialized!");

            ConnectionsListener.StopListening();
            foreach (var connection in Connections.Connections())
                connection.CloseConnection();
        }

        protected void NewConnectionHandler(INodeConnection nodeConnection)
        {

            nodeConnection.ConnectionClosed += Connections.RemoveConnection;
            nodeConnection.MessageReceived += NewMessageHandler;
            Connections.AddConnection(nodeConnection);
            NetworkExplorer.UpdateConnectionInfo(nodeConnection);
            nodeConnection.ListenMessages();
        }

        protected void NewMessageHandler(INodeConnection nodeConnection)
        {
            var message = nodeConnection.GetLastMessage();
            if (message == null)
                return;
            var msgContext = new MessageContext(message, nodeConnection);
            var msgPassMiddleware = ReceiveMiddlewareHead.Invoke(msgContext);
            if (msgPassMiddleware)
                MessageReceived?.Invoke(msgContext);
        }

        public int GetNodeTcpPort()
        {
            if (ConnectionsListener is NodeTcpListener listener)
                return listener.GetNodeTcpPort();
            throw new Exception("Um... Did I invent multiple listeners?");
        }

        public string GetNodeTcpIP()
        {
            throw new NotImplementedException();
        }
    }
}