using Akka.Actor;
using Akka.OpenTracing.Tracer;
using NSubstitute;
using NUnit.Framework;
using OpenTracing;
using OpenTracing.Propagation;

namespace Akka.OpenTracing.Tests.Tracer
{
    public class AkkaTracerTests
    {
        [Test]
        public void AroundReceive_SimpleSenderReceiver_SpanSetupCorrectly()
        {
            //setup builder and span
            var tracer = Substitute.For<ITracer>();            
            var spanBuilder = Substitute.For<ISpanBuilder>();
            tracer.BuildSpan(null).ReturnsForAnyArgs(spanBuilder);
            var scope = Substitute.For<IScope>();
            spanBuilder.StartActive(true).ReturnsForAnyArgs(scope);

            //setup akka context
            var context = Substitute.For<IActorContext>();
            var add = new Address("akka", "system");
            context.Self.Path.Returns(GetActorPath(add, "/ra"));
            context.Sender.Path.Returns(GetActorPath(add, "/sa"));            

            //call
            var at = new AkkaTracer(tracer, "akka://service");
            at.AroundReceive(context, new TracedMessage("m", null),typeof(SampleActor));

            //assert
            scope.Span.Received(1).SetTag("sender.path", "akka://system/sa");
            scope.Span.Received(1).SetTag("receiver.path", "akka://service/ra");
            scope.Span.Received(1).SetTag("receiver.type", "Akka.OpenTracing.Tests.SampleActor");
            scope.Span.Received(1).SetTag("message.type", "System.String");
        }

        [Test]
        public void AroundReceive_SpanMessageAvailable_SpanChildSetup()
        {
            //setup builder and span
            var tracer = Substitute.For<ITracer>();
            var spanBuilder = Substitute.For<ISpanBuilder>();
            tracer.BuildSpan(null).ReturnsForAnyArgs(spanBuilder);
            var scope = Substitute.For<IScope>();
            spanBuilder.StartActive(true).ReturnsForAnyArgs(scope);

            //setup akka context
            var context = Substitute.For<IActorContext>();
            var add = new Address("akka", "system");
            context.Self.Path.Returns(GetActorPath(add, "/ra"));
            context.Sender.Path.Returns(GetActorPath(add, "/sa"));

            //call
            var at = new AkkaTracer(tracer, "akka://service");
            at.AroundReceive(context, new TracedMessage("m", Substitute.For<ISpan>()), typeof(SampleActor));

            //assert
            spanBuilder.ReceivedWithAnyArgs(1).AsChildOf((ISpan)null);
        }

        [Test]
        public void AroundReceive_SpanMessageAvailable_SpanContextChildSetup()
        {
            //setup builder and span
            var tracer = Substitute.For<ITracer>();
            var spanBuilder = Substitute.For<ISpanBuilder>();
            tracer.BuildSpan(null).ReturnsForAnyArgs(spanBuilder);
            var scope = Substitute.For<IScope>();
            spanBuilder.StartActive(true).ReturnsForAnyArgs(scope);

            //setup akka context
            var context = Substitute.For<IActorContext>();
            var add = new Address("akka", "system");
            context.Self.Path.Returns(GetActorPath(add, "/ra"));
            context.Sender.Path.Returns(GetActorPath(add, "/sa"));

            //call
            var at = new AkkaTracer(tracer, "akka://service");
            var m = new TracedMessage("m", null);
            m.Context["a"] = "b";
            at.AroundReceive(context,m, typeof(SampleActor));

            //assert
            spanBuilder.ReceivedWithAnyArgs(1).AsChildOf((ISpanContext)null);
        }

        [Test]
        public void OnTell_SimpleCall_TracedMessageIsCorrectlyCreated()
        {
            //setup builder and span
            var tracer = Substitute.For<ITracer>();
            
            //setup akka context
            var receiver = Substitute.For<IActorRef>();
            var sender = Substitute.For<IActorRef>();
            var add = new Address("akka", "system");
            receiver.Path.Returns(GetActorPath(add, "/ra"));
            sender.Path.Returns(GetActorPath(add, "/sa"));

            //call
            var at = new AkkaTracer(tracer, "akka://service");            
            var trM = at.OnTell(receiver, sender, "m");

            //assert
            Assert.That(trM,Is.TypeOf<TracedMessage>());
            TracedMessage tm = (TracedMessage) trM;
            Assert.That(tm.Message, Is.EqualTo("m"));
            Assert.That(tm.Span, Is.Not.Null);
        }

        [Test]
        public void OnRemoteTell_SimpleCall_TracedMessageIsCorrectlyCreated()
        {
            //setup builder and span
            var tracer = Substitute.For<ITracer>();

            //setup akka context
            var receiver = Substitute.For<IActorRef>();
            var sender = Substitute.For<IActorRef>();
            var add = new Address("akka", "system");
            receiver.Path.Returns(GetActorPath(add, "/ra"));
            sender.Path.Returns(GetActorPath(add, "/sa"));

            //call
            var at = new AkkaTracer(tracer, "akka://service");
            var trM = at.OnRemoteTell(receiver, sender, "m");

            //assert
            Assert.That(trM, Is.TypeOf<TracedMessage>());
            TracedMessage tm = (TracedMessage)trM;
            Assert.That(tm.Message, Is.EqualTo("m"));
            Assert.That(tm.Span, Is.Null);
            tracer.ReceivedWithAnyArgs(1).Inject<ITextMap>(null,null,null);
        }

        [Test]
        public void OnEnqueue_TracedMessageAlreadySetup_SameMessageReturned()
        {
            //setup builder and span
            var tracer = Substitute.For<ITracer>();

            //setup akka context
            var receiver = Substitute.For<IActorRef>();
            var sender = Substitute.For<IActorRef>();
            var add = new Address("akka", "system");
            receiver.Path.Returns(GetActorPath(add, "/ra"));
            sender.Path.Returns(GetActorPath(add, "/sa"));

            //call
            var at = new AkkaTracer(tracer, "akka://service");
            var env = at.OnEnqueue(receiver, new Envelope(new TracedMessage("m",null),sender));

            //assert
            Assert.That(env.Message, Is.TypeOf<TracedMessage>());
            TracedMessage tm = (TracedMessage)env.Message;
            Assert.That(tm.Message, Is.EqualTo("m"));            
        }


        private ActorPath GetActorPath(Address add, string name)
        {
            if (string.IsNullOrEmpty(name))  return new RootActorPath(add);
            int sepIndex = name.LastIndexOf('/');
            return new ChildActorPath(GetActorPath(add, sepIndex == 0 ? "" : name.Substring(0,sepIndex-1)), name.Substring(sepIndex + 1),0);
        }
    }
}
