using System;

namespace Akka.OpenTracing.Tests
{
    public class SampleActor : TracedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case string s:
                    if (s == "exception")
                    {
                        throw new Exception();
                    }
                    Sender.Tell("received " + s, Self);
                    break;
            }
        }
    }
}