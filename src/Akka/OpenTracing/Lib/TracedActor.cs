using Akka.Actor;
using Akka.OpenTracing.Impl;
using Akka.OpenTracing.Tracer;
using OpenTracing;

namespace Akka.OpenTracing
{
    /// <summary>
    /// Any actor in the user space must implement it.
    /// </summary>
    public abstract class TracedActor : UntypedActor
    {
        private IAkkaTracer _tracer;

        protected override void PreStart()
        {
            _tracer = Context.System.GetExtension<TracingExtension>()?.AkkaTracer;
            base.PreStart();
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            IScope scope = null;
            try
            {
                if (message is TracedMessage m)
                {
                    scope = _tracer?.AroundReceive(Context, m, GetType());
                    message = m.Message;
                }
                
                return base.AroundReceive(receive, message);
            }
            finally
            {
                scope?.Dispose();
            }
        }
    }
}
