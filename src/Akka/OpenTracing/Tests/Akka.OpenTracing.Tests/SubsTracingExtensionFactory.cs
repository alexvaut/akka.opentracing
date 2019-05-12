using Akka.Actor;
using Akka.OpenTracing.Impl;
using Akka.OpenTracing.Tracer;

namespace Akka.OpenTracing.Tests
{
    internal class SubsTracingExtensionFactory : TracingExtensionFactory
    {
        private readonly IAkkaTracer _tracer;

        internal SubsTracingExtensionFactory(IAkkaTracer tracer)
        {
            _tracer = tracer;
        }

        public override TracingExtension CreateTracingExtension(ExtendedActorSystem system)
        {
            return base.CreateTracingExtension(_tracer);
        }
    }
}
