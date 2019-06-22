//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2018 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2018 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;

namespace HelloAkka
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.OpenTracing.TraceActorRefProvider, Akka.OpenTracing""
    }   
    extensions = [""Akka.OpenTracing.TracingExtensionProvider, Akka.OpenTracing""]

    tracing{
        server = ""a""
    }
}

traced-mailbox {
          mailbox-type = ""Akka.OpenTracing.TracedMailbox, Akka.OpenTracing""
        }
");

            // create a new actor system (a container for actors)
            var system = ActorSystem.Create("MySystem",config);

            // create actor and get a reference to it.
            // this will be an "ActorRef", which is not a 
            // reference to the actual actor instance
            // but rather a client or proxy to it
            var greeter = system.ActorOf<GreetingActor>("greeter");

            // send a message to the actor
            greeter.Tell(new Greet("World"));

            // prevent the application from exiting before message is handled
            Console.WriteLine("Wait for Ctrl+C.");
            ManualResetEvent e = new ManualResetEvent(false);
            Console.CancelKeyPress += delegate { e.Set(); };
            e.WaitOne();
        }
    }
}

