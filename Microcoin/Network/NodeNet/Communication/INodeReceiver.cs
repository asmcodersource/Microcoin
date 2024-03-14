namespace Microcoin.Network.NodeNet.Communication
{
    public interface INodeReceiver
    {
        Message.Message? GetLastMessage();
        List<Message.Message> GetMessageList();

    }
}
