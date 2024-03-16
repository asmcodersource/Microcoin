using Transaction;
using Microcoin.Network.NodeNet.Message;
using Newtonsoft.Json.Linq;

namespace Microcoin.Network.MessageAcceptors
{
    public class TransactionsAcceptor : IAcceptor
    {
        public event Action<Transaction.Transaction> TransactionReceived;

        public virtual async Task Handle(MessageContext messageContext)
        {
            JObject jsonRequestObject = JObject.Parse(messageContext.Message.Data);
            JToken? jsonTransactionToken = jsonRequestObject["transaction"];
            if (jsonTransactionToken is null)
                return;
            string transactionJsonString = jsonTransactionToken.ToString();
            Transaction.Transaction? transaction = Transaction.Transaction.ParseTransactionFromJson(transactionJsonString);
            if (transaction != null)
                TransactionReceived?.Invoke(transaction);
        }
    }
}
