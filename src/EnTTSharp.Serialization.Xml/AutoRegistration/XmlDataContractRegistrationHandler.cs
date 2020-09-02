using System.Reflection;
using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Xml.Impl;
using Serilog;

namespace EnTTSharp.Serialization.Xml.AutoRegistration
{
    public class XmlDataContractRegistrationHandler<TEntityKey> : EntityRegistrationHandlerBase where TEntityKey : IEntityKey
    {
        readonly ObjectSurrogateResolver objectResolver;
        static readonly ILogger Logger = Log.ForContext<XmlDataContractRegistrationHandler<TEntityKey>>();

        public XmlDataContractRegistrationHandler(ObjectSurrogateResolver objectResolver = null)
        {
            this.objectResolver = objectResolver;
        }

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            var componentType = typeof(TComponent);
            if (componentType.GetCustomAttribute<EntityXmlSerializationAttribute>() != null)
            {
                // Handled in the data processor for proper components.
                return;
            }

            var attr = componentType.GetCustomAttribute<DataContractAttribute>();
            if (attr == null)
            {
                return;
            }

            ReadHandlerDelegate<TComponent> readHandler = new DefaultDataContractReadHandler<TComponent>(objectResolver).Read;
            r.Store(XmlReadHandlerRegistration.Create(null, readHandler, false));
            WriteHandlerDelegate<TComponent> writeHandler = new DefaultDataContractWriteHandler<TComponent>(objectResolver).Write;
            r.Store(XmlWriteHandlerRegistration.Create(null, writeHandler, false));

            Logger.Debug("Registered Xml DataContract Handling for {ComponentType}", componentType);
        }
    }
}