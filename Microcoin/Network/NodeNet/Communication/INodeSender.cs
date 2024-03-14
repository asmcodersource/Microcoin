namespace Microcoin.Network.NodeNet.Communication
{
    public interface INodeSender
    {
        public void SendMessage(Message.Message message);
    }
}
