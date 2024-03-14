namespace Microcoin.PipelineHandling
{
    public class HandlePipeline<HandleType> : IHandlePipeline<HandleType>
    {
        protected List<IPipelineHandler<HandleType>> handlers = new List<IPipelineHandler<HandleType>>();

        public async Task<PipelineHandleResult<HandleType>> Handle(HandleType handleObject)
        {
            var handleResult = new PipelineHandleResult<HandleType>();
            handleResult.IsHandleSuccesful = false;
            foreach (var handler in handlers)
            {
                handleResult.LastExecutedHandle = handler;
                var handleSuccess = await handler.Handle(handleObject);
                if (handleSuccess is not true)
                    return handleResult;
            }
            handleResult.IsHandleSuccesful = true;
            return handleResult;
        }

        public void AddHandlerToPipeline(IPipelineHandler<HandleType> handler)
            => handlers.Add(handler);

        public bool RemoveHandlerFromPipeline(IPipelineHandler<HandleType> handler)
            => handlers.Remove(handler);
    }
}
