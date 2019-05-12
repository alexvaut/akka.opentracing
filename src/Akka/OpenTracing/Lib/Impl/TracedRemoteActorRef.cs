using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.OpenTracing.Tracer;
using Akka.Remote;
using Akka.Util;

namespace Akka.OpenTracing.Impl
{
    internal class TracedRemoteActorRef: IInternalActorRef, IRemoteRef
    {
        private readonly RemoteActorRef _delegate;
        private readonly IAkkaTracer _tracer;

        internal TracedRemoteActorRef(RemoteActorRef @delegate, IAkkaTracer tracer)
        {
            _delegate = @delegate;
            _tracer = tracer;
        }

        public int CompareTo(object obj)
        {
            return _delegate.CompareTo(obj);
        }

        public int CompareTo(IActorRef other)
        {
            return _delegate.CompareTo(other);
        }

        public bool Equals(IActorRef other)
        {
            return _delegate.Equals(other);
        }

        public IActorRef GetChild(IEnumerable<string> name)
        {
            return _delegate.GetChild(name);
        }

        public bool IsLocal => _delegate.IsLocal;

        public bool IsTerminated => _delegate.IsTerminated;

        public bool IsWatchIntercepted(IActorRef watchee, IActorRef watcher)
        {
            return _delegate.IsWatchIntercepted(watchee, watcher);
        }

        public Address LocalAddressToUse => _delegate.LocalAddressToUse;

        public IInternalActorRef Parent => _delegate.Parent;

        public ActorPath Path => _delegate.Path;

        public IActorRefProvider Provider => _delegate.Provider;

        public void Restart(Exception cause)
        {
            _delegate.Restart(cause);
        }

        public void Resume(Exception causedByFailure = null)
        {
            _delegate.Resume(causedByFailure);
        }

        public void SendSystemMessage(ISystemMessage message)
        {
            _delegate.SendSystemMessage(message);
        }

        public void SendSystemMessage(ISystemMessage message, IActorRef sender)
        {
            _delegate.SendSystemMessage(message, sender);
        }

        public void Start()
        {
            _delegate.Start();
        }

        public void Stop()
        {
            _delegate.Stop();
        }

        public void Suspend()
        {
            _delegate.Suspend();
        }

        public void Tell(object message, IActorRef sender)
        {
            message = _tracer.OnRemoteTell(this, sender, message);
            _delegate.Tell(message, sender);
        }

        public ISurrogate ToSurrogate(ActorSystem system)
        {
            return _delegate.ToSurrogate(system);
        }
    }
}
