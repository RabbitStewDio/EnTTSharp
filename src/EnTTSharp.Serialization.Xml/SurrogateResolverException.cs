using System;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization.Xml
{
    public class SurrogateResolverException: XmlReaderException
    {
        public SurrogateResolverException()
        {
        }

        protected SurrogateResolverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SurrogateResolverException(string message) : base(message)
        {
        }

        public SurrogateResolverException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SurrogateResolverException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}