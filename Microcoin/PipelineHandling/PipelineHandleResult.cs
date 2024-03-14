namespace Microcoin.PipelineHandling
{
    public record PipelineHandleResult<HandleType>
    {
        public IPipelineHandler<HandleType> LastExecutedHandle { get; set; } = null;
        public bool IsHandleSuccesful { get; set; } = false;
    }
}
