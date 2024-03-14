namespace Microcoin.PipelineHandling
{
    public class EmptyPipeline<HandleType> : IHandlePipeline<HandleType>
    {
        public void AddHandlerToPipeline(IPipelineHandler<HandleType> handler)
        {
            throw new NotImplementedException();
        }

        public Task<PipelineHandleResult<HandleType>> Handle(HandleType handleObject)
        {
            throw new NotImplementedException();
        }

        public bool RemoveHandlerFromPipeline(IPipelineHandler<HandleType> handler)
        {
            throw new NotImplementedException();
        }
    }
}
