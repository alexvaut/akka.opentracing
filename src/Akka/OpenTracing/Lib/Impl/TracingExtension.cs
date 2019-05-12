using Akka.Actor;
using Akka.OpenTracing.Tracer;

namespace Akka.OpenTracing.Impl
{
    public class TracingExtension : IExtension
    {
        internal IAkkaTracer AkkaTracer { get; }

        internal TracingExtension(IAkkaTracer akkaTracer)
        {
            AkkaTracer = akkaTracer;
        }
    }
}
