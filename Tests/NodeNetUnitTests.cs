using Microcoin.Network.NodeNet;
using Microcoin.RSAEncryptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{

    public class NodeNetConnection : IDisposable
    {
        public Node first_node { get; protected set; }
        public Node second_node { get; protected set; }

        public bool IsConnectionSuccess { get; protected set; }

        public NodeNetConnection()
        {
            first_node = Node.CreateRSAHttpNode(
                RSAEncryption.CreateSignOptions(),
                new Microcoin.Network.NodeNet.TcpCommunication.TcpListenerOptions(1333)
            );
            second_node = Node.CreateRSAHttpNode(
                RSAEncryption.CreateSignOptions(),
                new Microcoin.Network.NodeNet.TcpCommunication.TcpListenerOptions(1334)
            );
            IsConnectionSuccess = first_node.Connect("127.0.0.1:1334");
        }

        public void Dispose()
        {
            first_node.Close();
            second_node.Close();
        }
    }


    public class NodeNetUnitTests
    {
        static object wallLock = new object();

        [Fact]
        public void NodeNet_Communications_Messaging_SingleMessage_Test()
        {
            lock (wallLock)
            {
                using (var connections = new NodeNetConnection())
                {
                    Node first_node = connections.first_node;
                    Node second_node = connections.second_node;

                    string message = "Example message for testing";
                    int receivedMessagesCount = 0;
                    second_node.MessageReceived += (msgContext) =>
                    {
                        receivedMessagesCount++;
                        Assert.True(msgContext.Message.Data == message);
                    };
                    first_node.MessageReceived += (msgContext) =>
                    {
                        receivedMessagesCount++;
                        Assert.True(msgContext.Message.Data == message);
                    };

                    first_node.SendMessage(message);
                    second_node.SendMessage(message);

                    Thread.Sleep(50);

                    Assert.Equal(2, receivedMessagesCount);
                }
            }
        }


        [Fact]
        public void NodeNet_Communications_Messaging_MultipleMessage_Test()
        {
            lock(wallLock) 
            {
                using (var connections = new NodeNetConnection())
                {
                    Node first_node = connections.first_node;
                    Node second_node = connections.second_node;
                    int first_received_summary = 0;
                    int second_received_summary = 0;
                    int sending_summary = 0;
                    first_node.MessageReceived += (msgcontext) => { first_received_summary += Convert.ToInt32(msgcontext.Message.Data); };
                    second_node.MessageReceived += (msgcontext) => { second_received_summary += Convert.ToInt32(msgcontext.Message.Data); };
                    for (int i = 0; i < 1024; i++)
                    {
                        sending_summary += i;
                        first_node.SendMessage(i.ToString());
                        second_node.SendMessage((1023-i).ToString());
                    }

                    Thread.Sleep(100);

                    Assert.Equal(sending_summary, first_received_summary);
                    Assert.Equal(sending_summary, second_received_summary);
                }
            }
        }
    }
}
