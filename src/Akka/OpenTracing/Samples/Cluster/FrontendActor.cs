using Akka.Actor;
using System.Collections.Generic;

namespace Akka.OpenTracing.Sample.Cluster
{
    class FrontendActor : TracedActor
    {
        private readonly List<IActorRef> _backEnds;
        private IActorRef _storage;

        private int _jobCount;

        public FrontendActor()
        {            
            _backEnds = new List<IActorRef>();
        }

        protected override void OnReceive(object messageArg)
        {
            switch (messageArg)
            {
                case RegisterBackEnd _:
                    Context.Watch(Sender);
                    _backEnds.Add(Sender);
                    break;
                case RegisterStorage _:
                    _storage = Sender;
                    break;
                case JobRequest message:
                    _jobCount++;
                    Context.ActorOf(Props.Create(() => new WorkflowActor(_backEnds, _storage)), "workflow_" + _jobCount).Tell(message);
                    break;
                case Terminated message:
                    _backEnds.Remove(message.ActorRef);
                    break;
                default:
                    Unhandled(messageArg);
                    break;
            }
        }
    }
}
