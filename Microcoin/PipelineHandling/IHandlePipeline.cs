namespace Microcoin.PipelineHandling
{
    public interface IHandlePipeline<HandleType>
    {
        public Task<PipelineHandleResult<HandleType>> Handle(HandleType handleObject);
        public void AddHandlerToPipeline(IPipelineHandler<HandleType> handler);
        public bool RemoveHandlerFromPipeline(IPipelineHandler<HandleType> handler);
    }
}
