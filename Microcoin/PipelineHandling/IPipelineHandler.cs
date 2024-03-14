namespace Microcoin.PipelineHandling
{
    public interface IPipelineHandler<HandleType>
    {
        public Task<bool> Handle(HandleType handleObject);
    }
}
