using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Microcoin.UponNetwork.NetworkNode;
using TcpNetwork;
using Microcoin.Data;

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
                Console.WriteLine("Peer receive some message!");
                Message message = Message.Deserialize(receivedPacket.Data);
                switch (message.MessageType)
                {
                    case MessageType.CreateNewTransaction:
                        Console.WriteLine("Received Create new transaction message");
                        if (message.MessageObject as Transaction == null)
                            return;
                        ParentPeer.TransactionsPool.AddTransaction((Transaction)message.MessageObject);
                        break;
                    case MessageType.NewBlockMined:
                        if (message.MessageObject as Block == null)
                            return;
                        ParentPeer.Blockchain.NewBlockReceived((Block)message.MessageObject);
                        break;
                    case MessageType.NopeMessage:
                        Console.WriteLine("Someone sent nope message :D ");
                        break;

                    default:
                        Console.WriteLine("Someone sent wrong syntax message");
                        break;
                }
            } catch ( Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        } 
    }
}
