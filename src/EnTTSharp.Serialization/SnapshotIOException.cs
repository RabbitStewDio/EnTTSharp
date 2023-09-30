using System;
using System.IO;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization
{
    public class SnapshotIOException: IOException
    {
        public SnapshotIOException()
        {
        }

        protected SnapshotIOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SnapshotIOException(string message) : base(message)
        {
        }

        public SnapshotIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SnapshotIOException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}