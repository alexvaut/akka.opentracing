namespace Akka.OpenTracing.Sample.Cluster
{
    class RegisterBackEnd
    {
    }

    class RegisterStorage
    {
    }

    public class JobRequest : ContentMessage
    {
        public JobRequest(string content) : base(content){}
    }

    class JobAnswer : ContentMessage
    {
        public JobAnswer(string content) : base(content) { }
    }

    class ProcessRequest : ContentMessage
    {
        public ProcessRequest(string content) : base(content){}
    }

    class ProcessAnswer : ContentMessage
    {
        public ProcessAnswer(string content) : base(content){}
    }

    class StorageRequest : ContentMessage
    {
        public StorageRequest(string content) : base(content) { }
    }

    class StorageAnswer : ContentMessage
    {
        public StorageAnswer(string content) : base(content) { }
    }


    public class ContentMessage
    {        
        internal string Content { get; set;  }

        public ContentMessage(string content)
        {            
            Content = content;
        }

        public override string ToString()
        {
            return $"{GetType().Name}-'{Content}'";
        }
    }
}
