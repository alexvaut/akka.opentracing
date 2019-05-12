using Akka.Actor;
using Akka.OpenTracing.Tracer;
using Akka.Util;

namespace Akka.OpenTracing.Impl
{
    internal class TracedActorRef : IActorRef
    {
        private readonly IActorRefProvider _provider;
        private readonly IActorRef _delegate;
        private readonly IAkkaTracer _tracer;

        internal TracedActorRef(IActorRefProvider provider, IActorRef @delegate, IAkkaTracer tracer)
        {
            _provider = provider;
            _delegate = @delegate;
            _tracer = tracer;
        }

        public void Tell(object message, IActorRef sender)
        {            
            var newMessage = _tracer.OnTell(_delegate, sender, message);
            _delegate.Tell(newMessage, sender);
        }

        public bool Equals(IActorRef other)
        {
            return _delegate.Equals(other);
        }

        public int CompareTo(IActorRef other)
        {
            return _delegate.CompareTo(other);
        }

        public ISurrogate ToSurrogate(ActorSystem system)
        {
            return _delegate.ToSurrogate(system);
        }

        public int CompareTo(object obj)
        {
            return _delegate.CompareTo(obj);
        }

        public ActorPath Path => _delegate.Path;
    }
}
