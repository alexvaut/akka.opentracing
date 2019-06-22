using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Event;
using Akka.OpenTracing.Impl;
using Akka.OpenTracing.Tracer;

namespace Akka.OpenTracing
{
    /// <summary>
    /// Actor Ref provider to be used for local usage of akka only (no cluster or remote actors).
    /// </summary>
    public class TraceActorRefProvider : ITraceActorRefProvider
    {
        protected readonly IActorRefProvider _delegate;
        private ActorSystemImpl _system;
        private IAkkaTracer _tracer;

        public TraceActorRefProvider(string systemName, Settings settings, EventStream eventStream)
        {            
            _delegate = new LocalActorRefProvider(systemName, settings, eventStream);            
        }

        public TraceActorRefProvider(IActorRefProvider @delegate)
        {
            _delegate = @delegate;
        }

        internal IAkkaTracer Tracer
        {
            get
            {
                if (_tracer == null)
                {
                    _tracer = _system.GetExtension<TracingExtension>().AkkaTracer;
                }
                return _tracer;
            }
        }

        public IInternalActorRef RootGuardian => _delegate.RootGuardian;        

        public IActorRef RootGuardianAt(Address address)
        {
            return _delegate.RootGuardianAt(address);
        }

        public LocalActorRef Guardian => _delegate.Guardian;

        public LocalActorRef SystemGuardian => _delegate.SystemGuardian;

        public IActorRef DeadLetters => _delegate.DeadLetters;

        public ActorPath RootPath => _delegate.RootPath;

        public Settings Settings => _delegate.Settings;

        public void Init(ActorSystemImpl system)
        {
            _system = system;
            _delegate.Init(system);            
        }

        public Deployer Deployer => _delegate.Deployer;

        public ActorPath TempPath()
        {
            return _delegate.TempPath();
        }

        public IInternalActorRef TempContainer => _delegate.TempContainer;

        public void RegisterTempActor(IInternalActorRef actorRef, ActorPath path)
        {
            _delegate.RegisterTempActor(actorRef, path);
        }

        public void UnregisterTempActor(ActorPath path)
        {
            _delegate.UnregisterTempActor(path);
        }

        public IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor, ActorPath path,
            bool systemService, Deploy deploy, bool lookupDeploy, bool async)
        {            
            IInternalActorRef ret;
            if (typeof(TracedActor).IsAssignableFrom(props.Type))
            {                
                props = props.WithMailbox("traced-mailbox");
                ret = _delegate.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);                
            }
            else
            {
                ret = _delegate.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
            }

            return ret;
        }

        public IActorRef ResolveActorRef(string path)
        {            
            var ret = _delegate.ResolveActorRef(path);            
            return ret;
        }

        public IActorRef ResolveActorRef(ActorPath actorPath)
        {            
            var ret = _delegate.ResolveActorRef(actorPath);            
            return ret;
        }

        public Task TerminationTask => _delegate.TerminationTask;

        public Address GetExternalAddressFor(Address address)
        {
            return _delegate.GetExternalAddressFor(address);
        }

        public Address DefaultAddress => _delegate.DefaultAddress;
    }
}
