using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.Util.Internal;

namespace Akka.OpenTracing.Sample.Cluster
{
    class Program
    {
        private const string SystemName = "ClusterSystem";

        static void Main(string[] args)
        {
            //string role = args[0];
            //int port = int.Parse(args[1]);
            //Start(role, port);

            Start(2551, "backend", "storage");
            Start(2552, "backend");
            Start(2553, "frontend");
            //Start("storage", 2553);

            Console.WriteLine("Wait for Ctrl+C.");
            ManualResetEvent e = new ManualResetEvent(false);
            Console.CancelKeyPress += delegate { e.Set(); };
            e.WaitOne();
        }

        static void Start(int port, params string[] roles)
        {
            var config = GetConfig(roles, port);
            var system = ActorSystem.Create(SystemName, config);

            if (((IList) roles).Contains("backend"))
            {
                system.ActorOf(Props.Create(() => new ProcessActor()), "backend");
            }

            if (((IList) roles).Contains("storage"))
            {
                system.ActorOf(Props.Create(() => new StorageActor()), "storage");
            }

            if (((IList) roles).Contains("frontend"))
            {
                var frontend = system.ActorOf(Props.Create(() => new FrontendActor()), "frontend");
                var interval = TimeSpan.FromSeconds(2);
                var timeout = TimeSpan.FromSeconds(5);
                var counter = new AtomicCounter();

                //simulate users
                system.Scheduler.Advanced.ScheduleRepeatedly(interval, interval,
                    () => frontend.Ask(new JobRequest("job-" + counter.GetAndIncrement()), timeout)
                                  .ContinueWith(r => Console.WriteLine(r.Result)));
            }
        }

        private static Config GetConfig(string[] roles, int port)
        {
            var mainConfig = ConfigurationFactory.ParseString(@"
akka 
{  
    actor 
    {
        provider = ""Akka.OpenTracing.ClusterTraceActorRefProvider, Akka.OpenTracing""
    }   
    extensions = [""Akka.OpenTracing.TracingExtensionProvider, Akka.OpenTracing""]

    remote 
    {
        log-remote-lifecycle-events = DEBUG
        dot-netty.tcp 
        {
            hostname = ""0.0.0.0""
            public-hostname = ""localhost""
            port = 0            
        }
    }

    cluster 
    {
        seed-nodes = [""akka.tcp://ClusterSystem@localhost:2551""]
    }

    tracing 
    {
        server = ""a""
    }
}

traced-mailbox 
{
    mailbox-type = ""Akka.OpenTracing.TracedMailbox, Akka.OpenTracing""
}");

            var config =
                ConfigurationFactory.ParseString($"akka.remote.dot-netty.tcp.port={port}")
                    .WithFallback(ConfigurationFactory.ParseString($"akka.cluster.roles = [{string.Join(',', roles)}]"))
                    .WithFallback(mainConfig);
            return config;
        }
    }
}
