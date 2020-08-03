using System;
using System.Xml;

namespace EnTTSharp.Serialization.Xml.Impl
{
    public static class XmlReaderExtensions
    {
        public static void AdvanceToElement(this XmlReader reader, string localName = null)
        {
            while (reader.Read())
            {
                if (reader.EOF)
                {
                    throw new SnapshotIOException("EOF");
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (localName != null && reader.LocalName != localName)
                    {
                        throw new SnapshotIOException($"Expected {localName}, but found {reader.LocalName} instead.");
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

        public static bool TryGetAttributeInt(this XmlReader reader, string name, out int value)
        {
            string rawValue = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(rawValue))
            {
                value = default;
                return false;
            }

            return int.TryParse(rawValue, out value);
        }

        public static bool TryGetAttributeBool(this XmlReader reader, string name, out bool value)
        {
            string rawValue = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(rawValue))
            {
                value = default;
                return false;
            }

            return bool.TryParse(rawValue, out value);
        }

    }
}