using System.Collections.Generic;
using OpenTracing;

namespace Akka.OpenTracing.Tracer
{
    public class TracedMessage
    {
        public object Message { get; set; }
        public ISpan Span { get; set; }
        public Dictionary<string, string> Context { get; set;  }

        public TracedMessage(object message, ISpan span)
        {
            Message = message;
            Span = span;
            Context = new Dictionary<string, string>();
        }
    }
}
