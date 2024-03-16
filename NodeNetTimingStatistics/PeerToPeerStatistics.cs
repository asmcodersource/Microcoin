using BenchmarkDotNet.Attributes;
using Microcoin.Network.NodeNet;
using Microcoin.RSAEncryptions;
using System.Text;
using Tests.NodeNetNetworkConnections;


namespace NodeNetTimingStatistics
{
    public class PeerToPeerStatistics
    {
        string? data;
        Node first_node, second_node;
        NodeNetNetworkConnections nodeNetNetworkConnections = NodeNetNetworkConnections.Shared;

        [Params(512)]
        public int MessagesCount;

        [Params(256)]
        public int DataSize;

        

        [GlobalSetup]
        public void Setup()
        {
            do
            {
                first_node = nodeNetNetworkConnections.GetRandomNode();
                second_node = nodeNetNetworkConnections.GetRandomNode();
            } while (first_node == second_node);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < DataSize; i++)
                stringBuilder.Append((char)(Random.Shared.Next()));
            data = stringBuilder.ToString();
        }

        [Benchmark]
        public void SynchronousSending()
        {
            for (int i = 0; i < MessagesCount; i++)
                first_node.SendMessage(data).Wait();
        }

        [Benchmark]
        public void AsynchronousSending()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < MessagesCount; i++)
                tasks.Add(first_node.SendMessage(data));
            Task.WhenAll(tasks).Wait();
        }
    }
}
