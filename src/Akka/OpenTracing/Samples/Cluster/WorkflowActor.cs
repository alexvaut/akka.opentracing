using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Akka.OpenTracing.Sample.Cluster
{
    public class WorkflowActor : TracedActor
    {
        private readonly List<IActorRef> _backEnds;
        private readonly IActorRef _storage;

        private IActorRef _sender;
        private int _requestCount;
        private string _cumulResult;

        public WorkflowActor(List<IActorRef> backEnds, IActorRef storage)
        {                        
            _backEnds = backEnds;
            _storage = storage;            
        }

        protected override void OnReceive(object messageArg)
        {
            switch (messageArg)
            {
                case JobRequest request:
                    _sender = Sender;
                    _requestCount = new Random().Next(5);
                    _cumulResult = request.Content;
                    for (int i = 0; i < _requestCount; i++)
                    {
                        if (_backEnds.Any())
                        {
                            _backEnds[i % _backEnds.Count].Tell(new ProcessRequest($"{request.Content}_sub{i}"));
                        }
                    }
                    break;

                case ProcessAnswer _:
                    _requestCount--;
                    _cumulResult += $"_{_requestCount}";
                    if (_requestCount == 0)
                    {                        
                        _storage?.Tell(new StorageRequest($"{_cumulResult}_store"));
                    }
                    break;
                case StorageAnswer _:
                    _sender.Tell(new JobAnswer($"{_cumulResult}_ok"));
                    Context.Stop(Self);
                    break;
                default:
                    Unhandled(messageArg);
                    break;
            }
        }
    }
}
