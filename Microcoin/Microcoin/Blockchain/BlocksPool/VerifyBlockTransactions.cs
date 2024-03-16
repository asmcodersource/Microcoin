using Microcoin.PipelineHandling;


namespace BlocksPool
{
    public class VerifyBlockTransactions : IPipelineHandler<Block.Block>
    {
        protected readonly IHandlePipeline<Transaction.Transaction> transactionVerifyPipeline;

        public VerifyBlockTransactions(IHandlePipeline<Transaction.Transaction> transactionVerifyPipeline)
        {
            this.transactionVerifyPipeline = transactionVerifyPipeline;
        }

        public async Task<bool> Handle(Block.Block block)
        {
            foreach (var transaction in block.Transactions)
            {
                if (transaction.DateTime > block.MiningBlockInfo.CreateTime)
                    return false;
                var handleResult = await transactionVerifyPipeline.Handle(transaction);
                if (handleResult.IsHandleSuccesful is not true)
                    return false;
            }
            return true;
        }
    }
}
