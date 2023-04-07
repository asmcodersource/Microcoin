using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Microcoin.UponNetwork.NetworkNode;
using TcpNetwork;
using Microcoin.Data;
using System.Diagnostics;

namespace Microcoin.Peer
{
    public class MessageHandler
    {
        public Peer ParentPeer { get; protected set; }
        protected Node ParentNode { get; set; }
        protected XmlSerializer serializer;

        public MessageHandler(Peer parentPeer)
        {
            if (parentPeer.Node == null)
                throw new Exception("PeerPacketHandler cant use null node");

            this.serializer = new XmlSerializer(typeof(Message));
            this.ParentPeer = parentPeer;
            this.ParentNode = parentPeer.Node;
        }


        public void HandleMessage(object sender, ReceivedPacket receivedPacket)
        {
            // Deserialize packet, and verify receiver address
            try {
                Message message = Message.Deserialize(receivedPacket.Data);
                switch (message.MessageType)
                {
                    case MessageType.CreateNewTransaction:
                        if (message.MessageObject is not Transaction)
                            return;
                        ParentPeer.TransactionsPool.AddTransaction((Transaction)message.MessageObject);
                        break;
                    case MessageType.NewBlockMined:
                        if (message.MessageObject is not Block)
                            return;
                        
                        break;
                    case MessageType.NopeMessage:
                        break;
                }
            } catch ( Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        } 
    }
}
