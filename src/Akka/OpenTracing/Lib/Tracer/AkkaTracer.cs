using System;
using System.Threading;
using Akka.Actor;
using OpenTracing;
using OpenTracing.Propagation;

namespace Akka.OpenTracing.Tracer
{
    internal class AkkaTracer : IAkkaTracer
    {
        private readonly ITracer _tracer;
        private readonly string _serviceName;

        internal AkkaTracer(ITracer tracer, string serviceName)
        {
            _tracer = tracer;
            _serviceName = serviceName;
        }

        public object OnTell(IActorRef receiver, IActorRef sender, object message)
        {
            LocalLog($"!L {receiver.Path}.Tell({message})");
            AddSpan(receiver, sender, ref message);
            return message;
        }

        public object OnRemoteTell(IActorRef receiver, IActorRef sender, object message)
        {
            LocalLog($"!R {receiver.Path}.Tell({message})");
            AddSpanContext(receiver, sender, ref message);
            return message;
        }

        public IScope AroundReceive(IActorContext context, TracedMessage tracedMessage, Type actorType)
        {
            var actorPath = context.Self.Path.ToString().Substring(context.Self.Path.Address.ToString().Length);

            string operationName = $"{actorPath}:{tracedMessage.Message.GetType().Name}";            

            string parent = "no";             
            var builder = _tracer.BuildSpan(operationName);

            if (tracedMessage.Span != null)
            {
                parent = tracedMessage.Span.Context.SpanId;                
                builder = builder.AsChildOf(tracedMessage.Span);
            }
            else if (tracedMessage.Context.Count != 0)
            {
                ISpanContext spanContext = _tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(tracedMessage.Context));
                parent = spanContext.SpanId;
                builder = builder.AsChildOf(spanContext);
            }

            IScope ret = builder.StartActive(true);

            ret.Span.SetTag("sender.path", GetSenderPath(context.Sender));
            ret.Span.SetTag("receiver.path", GetLocalActorPath(context.Self));
            ret.Span.SetTag("receiver.type", actorType.ToString());
            ret.Span.SetTag("message.type", tracedMessage.Message.GetType().ToString());

            LocalLog($"> {context.Self.Path}.Tell({tracedMessage.Message}) Thread={Thread.CurrentThread.ManagedThreadId}, parent={parent}");

            return ret;
        }       

        public Envelope OnEnqueue(IActorRef receiver, Envelope envelope)
        {
            object message = envelope.Message;

            if (AddSpan(receiver, envelope.Sender, ref message))
            {
                envelope = new Envelope(message, envelope.Sender);
                LocalLog($"!@ {receiver.Path}.Tell({((TracedMessage)envelope.Message).Message}) Thread={Thread.CurrentThread.ManagedThreadId}");
            
            }            
            return envelope;
        }

        private string GetSenderPath(IActorRef sender)
        {
            string ret = sender.Path.ToString();
            if (!ret.Contains("@"))
            {
                ret = GetLocalActorPath(sender);
            }            
            return ret;
        }

        private string GetLocalActorPath(IActorRef actor)
        {
            string ret = _serviceName + "/" + string.Join("/", actor.Path.Elements);
            return ret;
        }
        
        private bool AddSpan(IActorRef receiver, IActorRef sender, ref object message)
        {            
            if (!(message is TracedMessage))
            {
                IScope scope = _tracer.ScopeManager.Active;
                message = new TracedMessage(message, scope?.Span);
                return true;
            }
            return false;
        }

        private bool AddSpanContext(IActorRef receiver, IActorRef sender, ref object message)
        {            
            IScope scope = _tracer.ScopeManager.Active;
            if (!(message is TracedMessage) && scope?.Span != null)
            {
                var tm = new TracedMessage(message, null);
                _tracer.Inject(scope.Span.Context, BuiltinFormats.TextMap, new TextMapInjectAdapter(tm.Context));
                message = tm;
                return true;
            }
            return false;
        }

        private void LocalLog(string message)
        {
            //Console.Out.WriteLine(message);
        }
    }
}
