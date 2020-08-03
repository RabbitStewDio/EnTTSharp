using System.Runtime.Serialization;
using System.Xml;

namespace EnTTSharp.Serialization.Xml.Impl
{
    public class DefaultDataContractWriteHandler<TData>
    {
        readonly DataContractSerializer serializer;

        public DefaultDataContractWriteHandler(ObjectSurrogateResolver surrogateResolver = null)
        {
            var ds = new DataContractSerializerSettings();
            ds.SerializeReadOnlyTypes = true;

            serializer = new DataContractSerializer(typeof(TData), ds);
            serializer.SetSerializationSurrogateProvider(surrogateResolver);
        }

        public void Write(XmlWriter writer, TData component)
        {
            serializer.WriteObject(writer, component);
        }
    }
}