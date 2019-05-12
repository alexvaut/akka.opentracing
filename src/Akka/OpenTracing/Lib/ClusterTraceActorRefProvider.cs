using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.OpenTracing.Impl;
using Akka.Remote;

namespace Akka.OpenTracing
{
    /// <summary>
    /// Cluster trace actor provider used when actors on a cluster are used.
    /// </summary>
    public class ClusterTraceActorRefProvider : TraceActorRefProvider, IClusterActorRefProvider
    {
        public ClusterTraceActorRefProvider(string systemName, Settings settings, EventStream eventStream) : 
            base(new ClusterActorRefProvider(systemName, settings, eventStream))
        {
        }

        private IClusterActorRefProvider Delegate => (IClusterActorRefProvider)_delegate;

        public bool HasAddress(Address address)
        {
            return Delegate.HasAddress(address);
        }

        public IActorRef InternalResolveActorRef(string path)
        {
            return Delegate.InternalResolveActorRef(path);
        }

        public Deploy LookUpRemotes(IEnumerable<string> p)
        {
            return Delegate.LookUpRemotes(p);
        }

        public void Quarantine(Address address, int? uid)
        {
            Delegate.Quarantine(address, uid);
        }

        public IInternalActorRef RemoteDaemon => Delegate.RemoteDaemon;

        public RemoteSettings RemoteSettings => Delegate.RemoteSettings;

        public IActorRef RemoteWatcher => Delegate.RemoteWatcher;

        public IInternalActorRef ResolveActorRefWithLocalAddress(string path, Address localAddress)
        {
            IInternalActorRef ret = Delegate.ResolveActorRefWithLocalAddress(path, localAddress);

            string localPath = new Uri(path).LocalPath;

            //WARNING: assumed any actor with root "/user/" is traced on the remote actor system.
            //If this is wrong the corresponding remote actor will receive directly a TracedMessage which it won't be able to decode.
            if (   localPath.StartsWith("/user")
                && ret is RemoteActorRef remoteActorRef)
            {                
                ret = new TracedRemoteActorRef(remoteActorRef, Tracer);
            }

            return ret;
        }

        public RemoteTransport Transport => Delegate.Transport;

        public void UseActorOnNode(RemoteActorRef actor, Props props, Deploy deploy, IInternalActorRef supervisor)
        {
            Delegate.UseActorOnNode(actor, props, deploy, supervisor);
        }
    }
}
