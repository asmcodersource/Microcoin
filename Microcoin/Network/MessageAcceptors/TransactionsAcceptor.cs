﻿using Microcoin.Network.NodeNet.Message;
using Microcoin.Blockchain.Transaction;
using Newtonsoft.Json.Linq;


namespace Microcoin.Network.MessageAcceptors
{
    internal class TransactionsAcceptor : IAcceptor
    {
        public event Action<Transaction> TransactionReceived;

        public virtual async Task Handle(MessageContext messageContext)
        {
            JObject jsonRequestObject = JObject.Parse(messageContext.Message.Data);
            JToken? jsonTransactionToken = jsonRequestObject["transaction"];
            if (jsonTransactionToken is null)
                return;
            string transactionJsonString = jsonTransactionToken.ToString();
            Transaction? transaction = Transaction.ParseTransactionFromJson(transactionJsonString);
            if (transaction != null)
                TransactionReceived?.Invoke(transaction);
        }
    }
}