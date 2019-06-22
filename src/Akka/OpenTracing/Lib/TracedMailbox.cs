using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;
using Akka.OpenTracing.Impl;

namespace Akka.OpenTracing
{
    /// <summary>
    /// Default mailbox for traced actor.
    /// Right now it's limited to the unbounded mail box.
    /// </summary>
    public class TracedMailbox : MailboxType, IProducesMessageQueue<IMessageQueue>
    {
        public TracedMailbox(Settings settings, Config config) : base(settings, config)
        {
        }

        public override IMessageQueue Create(IActorRef owner, ActorSystem system)
        {
            var tracer = system.GetExtension<TracingExtension>().AkkaTracer;
            var @delegate = new UnboundedMailbox(Settings, Config).Create(owner, system);
            return new TracedMessageQueue(@delegate, tracer);
        }
    }
}
