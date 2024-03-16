using Microcoin.PipelineHandling;

namespace BlocksPool
{
    public class BlocksPool
    {
        public event Action<BlocksPool, Block.Block>? OnBlockReceived;
        public IHandlePipeline<Block.Block> HandlePipeline { get; set; } = new EmptyPipeline<Block.Block>();
        public HashSet<Transaction.Transaction> PresentedTransactions { get; protected set; } = new HashSet<Transaction.Transaction>();

        public async Task HandleBlock(Block.Block block)
        {
            // Handle transaction on verifing pipeline
            var handleResult = await Task.Run(() => HandlePipeline.Handle(block));
            if (handleResult.IsHandleSuccesful is not true)
                return;
            // If transaction succesfully pass pipeline, add it to pool
            OnBlockReceived?.Invoke(this, block);
        }

        public void InitializeHandlerPipeline(IHandlePipeline<Transaction.Transaction> transactionVerifyPipeline)
        {
            HandlePipeline = new HandlePipeline<Block.Block>();
            HandlePipeline.AddHandlerToPipeline(new VerifyBlockFields());
            HandlePipeline.AddHandlerToPipeline(new VerifyBlockTransactions(transactionVerifyPipeline));
        }
    }
}
