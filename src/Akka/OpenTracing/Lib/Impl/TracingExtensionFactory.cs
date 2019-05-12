using Akka.Actor;
using Akka.OpenTracing.Tracer;

namespace Akka.OpenTracing.Impl
{
    internal class TracingExtensionFactory
    {
        public virtual TracingExtension CreateTracingExtension(ExtendedActorSystem system)
        {
            string addr = system.DeadLetters.Path.Address.ToString();
            if (system is ExtendedActorSystem eas)
            {
                addr = eas.Provider.DefaultAddress.ToString();
            }

            return CreateTracingExtension(new AkkaTracer(new TracingSettings(system.Settings.Config).Build(addr), addr));
        }

        public virtual TracingExtension CreateTracingExtension(IAkkaTracer akkaTracer)
        {
            return new TracingExtension(akkaTracer);
        }
    }
}
