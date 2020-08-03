using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public interface IXmlWriteHandler<TData>
    {
        void Initialize(ObjectSurrogateResolver surrogateResolver);
        void Write(XmlWriter writer, TData component);
    }
}