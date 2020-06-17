using System;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization.BinaryPack
{
    public class BinaryReaderException: Exception
    {
        public BinaryReaderException()
        {
        }

        protected BinaryReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BinaryReaderException(string message) : base(message)
        {
        }

        public BinaryReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}