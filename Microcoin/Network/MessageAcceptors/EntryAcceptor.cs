using Microcoin.Network.NodeNet.Message;
using Newtonsoft.Json.Linq;

namespace Microcoin.Network.MessageAcceptors
{
    public class EntryAcceptor : IAcceptor
    {
        public TransactionsAcceptor TransactionsAcceptor { get; protected set; }
        public BlocksAcceptor BlocksAcceptor { get; protected set; }

        public EntryAcceptor(TransactionsAcceptor transactionsAcceptor, BlocksAcceptor blocksAcceptor)
        {
            TransactionsAcceptor = transactionsAcceptor;
            BlocksAcceptor = blocksAcceptor;
        }

        public async Task Handle(MessageContext messageContext)
        {
            JObject jsonRequestObject = JObject.Parse(messageContext.Message.Data);
            if (jsonRequestObject["application"]?.ToString() != "Microcoin")
                return;
            switch (jsonRequestObject["type"]?.ToString())
            {
                case "WalletTransaction":
                    await TransactionsAcceptor.Handle(messageContext);
                    break;
                case "ChainBlock":
                    await BlocksAcceptor.Handle(messageContext);
                    break;
            }
        }
    }
}
