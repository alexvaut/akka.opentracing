using Akka.Configuration;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace Akka.OpenTracing.Tracer
{
    internal class TracingSettings
    {
        internal Config Config { get; private set; }

        internal TracingSettings(Config config)
        {
            Config = config;            
        }

        internal ITracer Build(string serviceName)
        {
            var loggerFactory = new LoggerFactory().AddConsole();

            //Very basic configuration
            var samplerConfiguration = new Jaeger.Configuration.SamplerConfiguration(loggerFactory)
            .WithType(ConstSampler.Type)
            .WithParam(1);

            //NOTE: not reporter for now
            var reporterConfiguration = new Jaeger.Configuration.ReporterConfiguration(loggerFactory)
                .WithLogSpans(true);

            //NOTE: only support jaeger for now
            var config = Jaeger.Configuration.FromEnv(loggerFactory);
            return config.GetTracer();

            //return new Jaeger.Configuration(serviceName, loggerFactory)
            //                 .WithSampler(samplerConfiguration)        
            //                 .WithReporter(reporterConfiguration)                             
            //                 .GetTracer();
        }
    }
}
