using Microcoin.Blockchain.Transaction;
using Microcoin.PipelineHandling;
using Microcoin.RSAEncryptions;


namespace Microcoin.Blockchain.TransactionsPool
{
    public class VerifyTransactionSign : IPipelineHandler<Microcoin.Blockchain.Transaction.Transaction>
    {
        public async Task<bool> Handle(Microcoin.Blockchain.Transaction.Transaction transaction)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            IReceiverSignOptions receiverSignOptions = TransactionValidator.GetReceiverValidateOptions(transaction);
            ITransactionValidator transactionValidator = new TransactionValidator();
            transactionValidator.SetValidateOptions(receiverSignOptions);
            bool isSignValid = transactionValidator.Validate(transaction);
            return isSignValid;
        }
    }
}
