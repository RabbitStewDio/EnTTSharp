using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlReaderExceptionExtensions
    {
        public static XmlReaderException FromMissingAttribute(this XmlReader reader, string tag, string attribute)
        {
            if (reader is IXmlLineInfo li && li.HasLineInfo())
            {
                return new XmlReaderException($"Missing attribute '{attribute}' on element '{tag}' at {li.LineNumber}:{li.LinePosition}");
            }
            return new XmlReaderException($"Missing attribute '{attribute}' on element '{tag}'");
        }
    }
}