using Akka.Actor;
using Akka.OpenTracing.Impl;

namespace Akka.OpenTracing
{
    /// <summary>Tracing Extension Provider</summary>
    public class TracingExtensionProvider : ExtensionIdProvider<TracingExtension>
    {
        private readonly TracingExtensionFactory _factory;

        internal TracingExtensionProvider(TracingExtensionFactory factory)
        {
            _factory = factory;
        }

        public TracingExtensionProvider() : this(new TracingExtensionFactory())
        {
        }

        public override TracingExtension CreateExtension(ExtendedActorSystem system)
        {
            return _factory.CreateTracingExtension(system);
        }
    }
}
