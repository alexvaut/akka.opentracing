using Akka.Actor;
using Akka.Dispatch.MessageQueues;
using Akka.OpenTracing.Tracer;

namespace Akka.OpenTracing.Impl
{
    internal class TracedMessageQueue : IMessageQueue
    {
        private readonly IMessageQueue _delegate;
        private readonly IAkkaTracer _tracer;

        internal TracedMessageQueue(IMessageQueue @delegate, IAkkaTracer tracer)
        {
            _delegate = @delegate;
            _tracer = tracer;
        }

        public bool HasMessages => _delegate.HasMessages;

        public int Count => _delegate.Count;

        public void Enqueue(IActorRef receiver, Envelope envelope)
        {
            envelope = _tracer.OnEnqueue(receiver, envelope);
            _delegate.Enqueue(receiver, envelope);
        }

        public bool TryDequeue(out Envelope envelope)
        {
            return _delegate.TryDequeue(out envelope);
        }

        public void CleanUp(IActorRef owner, IMessageQueue deadletters)
        {
            _delegate.CleanUp(owner, deadletters);
        }
    }
}
