using System.Reflection;
using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using Serilog;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlDataContractRegistrationHandler : EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<XmlDataContractRegistrationHandler>();

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

            ReadHandlerDelegate<TComponent> readHandler = new DefaultDataContractReadHandler<TComponent>().Read;
            r.Store(XmlReadHandlerRegistration.Create("", readHandler, false));
            WriteHandlerDelegate<TComponent> writeHandler = new DefaultDataContractWriteHandler<TComponent>().Write;
            r.Store(XmlWriteHandlerRegistration.Create("", writeHandler, false));

            Logger.Debug("Registered Xml DataContract Handling for {ComponentType}", componentType);
        }
    }
}