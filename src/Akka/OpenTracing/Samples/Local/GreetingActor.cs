//-----------------------------------------------------------------------
// <copyright file="GreetingActor.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2018 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2018 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Actor;
using System;
using System.Threading;
using Akka.OpenTracing;

namespace HelloAkka
{
    /// <summary>
    /// The actor class
    /// </summary>
    public class GreetingActor : TracedActor
    {
        private IActorRef f1, f2, f3;

        public GreetingActor()
        {
            f1 = Context.ActorOf(new Props(typeof(FollowActor)), "follow1");
            f2 = Context.ActorOf(new Props(typeof(FollowActor)), "follow2");
            f3 = Context.ActorOf(new Props(typeof(FollowActor)), "follow3");
        }

        protected override void OnReceive(object message)
        {
            Console.WriteLine($"GreetingActor.OnReceive Thread={Thread.CurrentThread.ManagedThreadId}");
            switch (message)
            {
                case Greet greet:
                    Console.WriteLine("Hello {0}", greet.Who);
                    Thread.Sleep(20);                    
                    f1.Tell("test1");
                    f2.Tell("test2");
                    f3.Tell("test3");
                    break;
                default:
                    Sender.Tell(0);
                    Thread.Sleep(40);
                    break;
            }
        }
    }

    public class FollowActor : TracedActor
    {
        protected override void OnReceive(object message)
        {
            Console.WriteLine($"FollowActor.OnReceive Thread={Thread.CurrentThread.ManagedThreadId}");

            switch (message)
            {
                case string m:
                    Thread.Sleep(30);
                    
                    Sender.Tell(m + "_answer",Self);
                    break;
            }
        }
    }


}

