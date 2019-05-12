using Akka.Actor;
using Akka.OpenTracing.Tracer;
using Akka.TestKit;
using NSubstitute;
using NUnit.Framework;
using OpenTracing;

namespace Akka.OpenTracing.Tests
{
    public class TracedActorTests : TestKit.NUnit3.TestKit
    {
        [Test]
        public void Tell_NormalMessage_Received()
        {
            //setup
            var testActor = Sys.ActorOf(Props.Create(() => new SampleActor()));

            //call
            testActor.Tell("a");

            //assert
            var receivedMessage = ExpectMsg<string>();
            Assert.That(receivedMessage, Is.EqualTo("received a"));
        }

        [Test]
        public void Tell_TracedMessage_Received()
        {
            //setup
            var testActor = new TestActorRef<SampleActor>(Sys, Props.Create(() => new SampleActor())); //sync actor

            //call
            testActor.Tell(new TracedMessage("a",null));

            //assert
            var receivedMessage = ExpectMsg<string>();
            Assert.That(receivedMessage, Is.EqualTo("received a"));
        }

        [Test]
        public void Tell_TracedMessage_TracerAroundReceiveCalled()
        {
            //setup
            IAkkaTracer tracer = Substitute.For<IAkkaTracer>();
            IScope scope = Substitute.For<IScope>();
            tracer.AroundReceive(null, null, null).ReturnsForAnyArgs(scope);

            var tep = new TracingExtensionProvider(new SubsTracingExtensionFactory(tracer));
            Sys.RegisterExtension(tep);
            var testActor = new TestActorRef<SampleActor>(Sys, Props.Create(() => new SampleActor())); //sync actor

            //call
            testActor.Tell(new TracedMessage("exception", null));

            //assert            
            tracer.ReceivedWithAnyArgs(1).AroundReceive(null, null, null);
        }

        [Test]
        public void Tell_TracedMessage_TracerScopeDisposed()
        {
            //setup
            IAkkaTracer tracer = Substitute.For<IAkkaTracer>();
            IScope scope = Substitute.For<IScope>();
            tracer.AroundReceive(null, null, null).ReturnsForAnyArgs(scope);

            var tep = new TracingExtensionProvider(new SubsTracingExtensionFactory(tracer));
            Sys.RegisterExtension(tep);
            var testActor = new TestActorRef<SampleActor>(Sys, Props.Create(() => new SampleActor())); //sync actor

            //call
            testActor.Tell(new TracedMessage("a", null));

            //assert            
            scope.ReceivedWithAnyArgs(1).Dispose();
        }

        [Test]
        public void Tell_TracedMessageAndExceptionDuringProcessing_TracerScopeDisposed()
        {
            //setup
            IAkkaTracer tracer = Substitute.For<IAkkaTracer>();
            IScope scope = Substitute.For<IScope>();
            tracer.AroundReceive(null, null, null).ReturnsForAnyArgs(scope);

            var tep = new TracingExtensionProvider(new SubsTracingExtensionFactory(tracer));
            Sys.RegisterExtension(tep);
            var testActor = new TestActorRef<SampleActor>(Sys, Props.Create(() => new SampleActor())); //sync actor

            //call
            testActor.Tell(new TracedMessage("exception", null));

            //assert            
            scope.ReceivedWithAnyArgs(1).Dispose();            
        }
    }
}