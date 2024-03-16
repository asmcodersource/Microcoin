using Transaction;
using Microcoin.PipelineHandling;
using Microcoin.RSAEncryptions;


namespace TransactionsPool
{
    public class VerifyTransactionSign : IPipelineHandler<Transaction.Transaction>
    {
        public async Task<bool> Handle(Transaction.Transaction transaction)
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
