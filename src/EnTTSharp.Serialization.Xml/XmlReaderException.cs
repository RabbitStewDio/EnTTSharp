using System;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlReaderException: SnapshotIOException
    {
        public XmlReaderException()
        {
        }

        protected XmlReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public XmlReaderException(string message) : base(message)
        {
        }

        public XmlReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public XmlReaderException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}