﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcoin.PipelineHandling;
using Microcoin.Transaction;
using Microcoin.TransactionPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microcoin.TransactionPool
{
    internal class TransactionsPool
    {
        public event Action<TransactionsPool> OnTransactionReceived;
        public List<Transaction.Transaction> Pool { get; protected set; } = new List<Transaction.Transaction>();
        public HashSet<Transaction.Transaction> PresentedTransactions { get; protected set; } = new HashSet<Transaction.Transaction>();
        public IHandlePipeline<Transaction.Transaction> HandlePipeline { get; set; } = new EmptyPipeline<Transaction.Transaction>();

        public List<Transaction.Transaction> TakeTransactions()
        {
            var transactions = Pool.ToList();
            return transactions;
        }

        public void RemoveTransactions(List<Transaction.Transaction> removeTransactions)
        {
            // Some transaction can be expired after some events
            // ( for example being include in some block already )
            // Performed without lock, resposibility lies on controll code
            foreach (var transaction in removeTransactions)
                RemoveTransaction(transaction);
        }

        public async Task HandleTransactionMessage(Transaction.Transaction transaction)
        {      
            // Handle transaction on verifing pipeline
            var handleResult = await Task.Run(() => HandlePipeline.Handle(transaction));
            if (handleResult.IsHandleSuccesful is not true)
                return;
            // If transaction succesfully pass pipeline, add it to pool
            AddTransaction(transaction);
        }

        public void InitializeHandlerPipeline()
        {
            HandlePipeline = new HandlePipeline<Transaction.Transaction>();
            HandlePipeline.AddHandlerToPipeline(new VerifyTransactionSign());
            HandlePipeline.AddHandlerToPipeline(new VerifyTransactionFields());
            HandlePipeline.AddHandlerToPipeline(new VerifyTransactionDuplication());
        }

        protected void AddTransaction(Transaction.Transaction transaction)
        {
            lock (this)
            {
                if (PresentedTransactions.Contains(transaction))
                    return;
                Pool.Add(transaction);
                PresentedTransactions.Add(transaction);
                OnTransactionReceived?.Invoke(this);
            }
        }

        protected void RemoveTransaction(Transaction.Transaction transaction)
        {
            lock (this)
            {
                PresentedTransactions.Remove(transaction);
                Pool.Remove(transaction);
            }
        }
    }
}
