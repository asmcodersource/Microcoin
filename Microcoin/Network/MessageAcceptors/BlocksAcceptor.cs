using Microcoin.Network.NodeNet.Message;
using Newtonsoft.Json.Linq;

namespace Microcoin.Network.MessageAcceptors
{
    public class BlocksAcceptor : IAcceptor
    {
        public event Action<Microcoin.Blockchain.Block.Block> BlockReceived;

        public virtual async Task Handle(MessageContext messageContext)
        {
            JObject jsonRequestObject = JObject.Parse(messageContext.Message.Data);
            JToken? jsonBlockToken = jsonRequestObject["block"];
            if (jsonBlockToken is null)
                return;
            string blockJsonString = jsonBlockToken.ToString();
            Microcoin.Blockchain.Block.Block? block = Microcoin.Blockchain.Block.Block.ParseBlockFromJson(blockJsonString);
            if (block != null)
                BlockReceived?.Invoke(block);
        }
    }
}
