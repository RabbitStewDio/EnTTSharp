using System.Xml;
using System.Xml.Serialization;

namespace EnTTSharp.Serialization.Xml.Impl
{
    public class DefaultWriteHandler<T>
    {
        readonly XmlSerializer serializer;

        public DefaultWriteHandler()
        {
            serializer = new XmlSerializer(typeof(T));
            
        }

        public void Write(XmlWriter writer, T component)
        {
            object? o = component;
            if (o != null)
            {
                serializer.Serialize(writer, o);
            }
        }
    }
}