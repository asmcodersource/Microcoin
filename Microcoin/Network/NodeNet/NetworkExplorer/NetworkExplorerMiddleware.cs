﻿using Microcoin.Network.NodeNet.Message;
using Microcoin.Network.NodeNet.NetworkExplorer.Requests;
using Microcoin.Network.NodeNet.NetworkExplorer.Responses;
using Microcoin.Network.NodeNet.ReceiveMiddleware;
using Newtonsoft.Json;

namespace Microcoin.Network.NodeNet.NetworkExplorer
{
    public class NetworkExplorerMiddleware : IReceiveMiddleware
    {
        public NetworkExplorer Explorer { get; protected set; }
        public Node Node { get; protected set; }
        public IReceiveMiddleware NextReceiveMiddleware { get; protected set; }
        public IReceiveMiddleware Next { get; protected set; } = null;

        public NetworkExplorerMiddleware(Node node, NetworkExplorer explorer)
        {
            // would be used to store new connections
            Node = node;
            Explorer = explorer;
        }

        public bool Invoke(MessageContext messageContext)
        {
            if (messageContext.Message.Info.IsTechnical != true)
                return Next == null ? true : Next.Invoke(messageContext);
            AcceptExporerMessages(messageContext);
            return Next == null ? true : Next.Invoke(messageContext);
        }

        public void SetNext(IReceiveMiddleware next)
        {
            Next = next;
        }

        public void AcceptExporerMessages(MessageContext messageContext)
        {
            string requestJson = messageContext.Message.Data.ToString();
            var rawMessage = JsonConvert.DeserializeObject<dynamic>(requestJson);
            var receivedType = (string)rawMessage.MessageType;
            var messageType = Type.GetType(receivedType);
            var message = JsonConvert.DeserializeObject(requestJson, messageType);

            if (message is IRequest request)
            {
                switch (request)
                {
                    case EchoRequest echoRequest:
                        // Should be ok?
                        Explorer.UpdateConnectionInfo(echoRequest.MyAddress);
                        var echoRequestResponse = new EchoResponse();
                        echoRequestResponse.MyAddress = messageContext.SenderConnection.GetConnectionAddress();
                        Node.SendMessage(JsonConvert.SerializeObject(echoRequestResponse), messageContext.Message.Info.ReceiverPublicKey, true);
                        break;
                };
            }
            if (message is IResponse response)
            {
                switch (response)
                {
                    case EchoResponse echoResponse:
                        Explorer.UpdateConnectionInfo(echoResponse.MyAddress);
                        break;
                };
            }
        }
    }
}
