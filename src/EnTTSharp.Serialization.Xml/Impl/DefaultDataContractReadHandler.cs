using System.Runtime.Serialization;
using System.Xml;

namespace EnTTSharp.Serialization.Xml.Impl
{
    public class DefaultDataContractReadHandler<TComponent>
    {
        readonly DataContractSerializer serializer;

        public DefaultDataContractReadHandler(ObjectSurrogateResolver objectResolver = null)
        {
            var serializerSettings = new DataContractSerializerSettings()
            {
                SerializeReadOnlyTypes = true
            };

            serializer = new DataContractSerializer(typeof(TComponent), serializerSettings);
            serializer.SetSerializationSurrogateProvider(objectResolver);
        }

        public TComponent Read(XmlReader reader)
        {
            return (TComponent)serializer.ReadObject(reader);
        }
    }
}