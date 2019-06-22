using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;

namespace Akka.OpenTracing.Sample.Cluster
{
    public class StorageActor : TracedActor
    {
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);

        /// <summary>
        /// Need to subscribe to cluster changes
        /// </summary>
        protected override void PreStart()
        {
            base.PreStart();
            Cluster.Subscribe(Self, typeof(ClusterEvent.MemberUp));
        }

        /// <summary>
        /// Re-subscribe on restart
        /// </summary>
        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        protected override void OnReceive(object messageArg)
        {
            switch (messageArg)
            {
                case StorageRequest message:
                    Thread.Sleep(new Random().Next(2000));
                    Sender.Tell(new StorageAnswer($"{message.Content}_stored."));
                    break;
                case ClusterEvent.CurrentClusterState message:
                    var state = message;
                    foreach (var member in state.Members)
                    {
                        if (member.Status == MemberStatus.Up)
                        {
                            Register(member);
                        }
                    }

                    break;
                case ClusterEvent.MemberUp message:
                    var memUp = message;
                    Register(memUp.Member);
                    break;
                default:
                    Unhandled(messageArg);
                    break;
            }
        }

        protected void Register(Member member)
        {
            if (member.HasRole("frontend"))
            {
                Context.ActorSelection(member.Address + "/user/frontend").Tell(new RegisterStorage());
            }
        }
    }
}
