using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.OpenTracing.Tracer;
using Akka.Util;

namespace Akka.OpenTracing.Impl
{
    internal class InternalTracedActorRef : IInternalActorRef
    {
        private readonly IActorRefProvider _provider;
        private readonly IInternalActorRef _delegate;
        private readonly IAkkaTracer _tracer;        

        internal InternalTracedActorRef(IActorRefProvider provider, IInternalActorRef @delegate, IAkkaTracer tracer)
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

        public bool IsLocal => _delegate.IsLocal;

        public IInternalActorRef Parent => _delegate.Parent;

        public IActorRefProvider Provider => _provider;

        [Obsolete]
        public bool IsTerminated => _delegate.IsTerminated;

        public IActorRef GetChild(IEnumerable<string> name)
        {
            return _delegate.GetChild(name);
        }

        public void Resume(Exception causedByFailure = null)
        {
            _delegate.Resume(causedByFailure);
        }

        public void Start()
        {
            _delegate.Start();
        }

        public void Stop()
        {
            _delegate.Stop();
        }

        public void Restart(Exception cause)
        {
            _delegate.Restart(cause);
        }

        public void Suspend()
        {
            _delegate.Suspend();
        }

        [Obsolete]
        public void SendSystemMessage(ISystemMessage message, IActorRef sender)
        {
            _delegate.SendSystemMessage(message, sender);
        }

        public void SendSystemMessage(ISystemMessage message)
        {
            _delegate.SendSystemMessage(message);
        }


    }
}
