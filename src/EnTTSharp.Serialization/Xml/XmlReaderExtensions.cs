using System;
using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlReaderExtensions
    {
        public static void AdvanceToElement(this XmlReader reader, string localName = null)
        {
            while (reader.Read())
            {
                if (reader.EOF)
                {
                    throw new XmlException("EOF");
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (localName != null && reader.LocalName != localName)
                    {
                        throw new XmlException($"Expected {localName}, but found {reader.LocalName} instead.");
                    }

                    return;
                }
            }
        }

        public static void ReadChildElements(this XmlReader reader, Func<XmlReader, bool> onStartElement)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (!onStartElement(reader))
                        {
                            reader.Skip();
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        public static void ReadChildElements(this XmlReader reader, Func<bool> onStartElement)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (!onStartElement())
                        {
                            reader.Skip();
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

    }
}