using System;
using Akka.Actor;
using OpenTracing;

namespace Akka.OpenTracing.Tracer
{
    internal interface IAkkaTracer
    {
        object OnTell(IActorRef receiver, IActorRef sender, object message);

        object OnRemoteTell(IActorRef receiver, IActorRef sender, object message);

        IScope AroundReceive(IActorContext context, TracedMessage tracedMessage, Type actorType);

        Envelope OnEnqueue(IActorRef receiver, Envelope envelope);        
    }
}
