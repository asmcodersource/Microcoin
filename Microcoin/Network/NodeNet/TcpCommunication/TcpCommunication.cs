using Microcoin.Network.NodeNet.Communication;

namespace Microcoin.Network.NodeNet.TcpCommunication
{
    public class TcpCommunication : INodeConnections
    {
        List<INodeConnection> nodeHttpConnections = new List<INodeConnection>();

        public void AddConnection(INodeConnection connection)
        {
            nodeHttpConnections.Add(connection);
        }

        public void RemoveConnection(INodeConnection connection)
        {
            nodeHttpConnections.Remove(connection);
        }

        public List<INodeConnection> Connections()
        {
            return new List<INodeConnection>(nodeHttpConnections);
        }
    }
}
