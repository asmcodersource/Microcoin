namespace Microcoin.Network.NodeNet.Communication
{
    public interface INodeSender
    {
        public Task SendMessage(Message.Message message);
    }
}
