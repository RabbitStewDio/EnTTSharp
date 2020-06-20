using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using Serilog;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlEntityRegistrationHandler: EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<XmlEntityRegistrationHandler>();

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            var componentType = typeof(TComponent);
            var attr = componentType.GetCustomAttribute<EntityXmlSerializationAttribute>();
            if (attr == null)
            {
                return;
            }

            bool hasReadHandler = false;
            bool hasWriteHandler = false;
            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in handlerMethods)
            {
                if (IsXmlReader<TComponent>(m))
                {
                    hasReadHandler = true;
                    var d = (ReadHandlerDelegate<TComponent>) Delegate.CreateDelegate(typeof(ReadHandlerDelegate<TComponent>), null, m, false);
                    r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, d, attr.UsedAsTag));
                }

                if (IsXmlWriter<TComponent>(m))
                {
                    hasWriteHandler = true;
                    var d = (WriteHandlerDelegate<TComponent>)Delegate.CreateDelegate(typeof(WriteHandlerDelegate<TComponent>), null, m, false);
                    r.Store(XmlWriteHandlerRegistration.Create(attr.ComponentTypeId, d, attr.UsedAsTag));
                }
            }

            if (!hasReadHandler)
            {
                if (HasDataContract<TComponent>())
                {
                    ReadHandlerDelegate<TComponent> handler = new DefaultDataContractReadHandler<TComponent>().Read;
                    r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, handler, attr.UsedAsTag));
                }
                else
                {
                    ReadHandlerDelegate<TComponent> handler = new DefaultReadHandler<TComponent>().Read;
                    r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, handler, attr.UsedAsTag));
                }
            }

            if (!hasWriteHandler)
            {
                if (HasDataContract<TComponent>())
                {
                    WriteHandlerDelegate<TComponent> handler = new DefaultDataContractWriteHandler<TComponent>().Write;
                    r.Store(XmlWriteHandlerRegistration.Create(attr.ComponentTypeId, handler, attr.UsedAsTag));
                }
                else
                {
                    WriteHandlerDelegate<TComponent> handler = new DefaultWriteHandler<TComponent>().Write;
                    r.Store(XmlWriteHandlerRegistration.Create(attr.ComponentTypeId, handler, attr.UsedAsTag));
                }
            }

            Logger.Debug("Registered Xml Handling for {ComponentType}", componentType);

        }

        bool HasDataContract<TComponent>()
        {
            var componentType = typeof(TComponent);
            return componentType.GetCustomAttribute<DataContractAttribute>() != null;
        }

        bool IsXmlReader<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlReaderAttribute>() != null
                   && methodInfo.IsSameFunction(componentType, typeof(XmlReader));
        }

        bool IsXmlWriter<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlWriterAttribute>() != null
                   && methodInfo.IsSameAction(typeof(XmlWriter), componentType);
        }

    }
}