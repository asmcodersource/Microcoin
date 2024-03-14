using Microcoin.Network.NodeNet.Message;

namespace Microcoin.Network.Sessions
{
    /// <summary>
    /// Implements communication across different sessions on top of one communication stream. 
    /// Allows you to conduct multiple threads of communication simultaneously.
    /// </summary>
    public class Session
    {
        protected Queue<MessageContext> MessageContexts = new Queue<MessageContext>();
        protected event Action<MessageContext> onMessageContextReceive = null;

        public short LocalSideId { get; protected set; }
        public short RemoteSideId { get; protected set; }


        public Session(short localSideId, short remoteSideId)
        {
            LocalSideId = localSideId;
            RemoteSideId = remoteSideId;
        }

        public void AddMessageContextToQueue(MessageContext MessageContext)
        {
            lock (MessageContexts)
            {
                MessageContexts.Enqueue(MessageContext);
                onMessageContextReceive?.Invoke(MessageContext);
            }
        }

        public async Task<MessageContext> GetMessageContext(CancellationToken token)
        {
            var taskCompletionSource = new TaskCompletionSource<MessageContext>();
            try
            {
                lock (MessageContexts)
                {
                    if (MessageContexts.Count != 0)
                        return MessageContexts.Dequeue();
                    onMessageContextReceive += (MessageContext) =>
                    {
                        onMessageContextReceive = null;
                        taskCompletionSource.SetResult(MessageContexts.Dequeue());
                    };
                }
                return await taskCompletionSource.Task;
            }
            finally
            {
                onMessageContextReceive = null;
            }
        }
    }
}
