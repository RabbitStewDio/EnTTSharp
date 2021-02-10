using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnTTSharp.Serialization.Xml.Impl;
using Serilog;

namespace EnTTSharp.Serialization.Xml.AutoRegistration
{
    public class XmlEntityRegistrationHandler: EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<XmlEntityRegistrationHandler>();
        readonly ObjectSurrogateResolver objectResolver;

        public XmlEntityRegistrationHandler(ObjectSurrogateResolver objectResolver = null)
        {
            this.objectResolver = objectResolver;
        }

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            var componentType = typeof(TComponent);
            var attr = componentType.GetCustomAttribute<EntityXmlSerializationAttribute>();
            if (attr == null)
            {
                return;
            }

            ReadHandlerDelegate<TComponent> readHandler = null;
            WriteHandlerDelegate<TComponent> writeHandler = null;
            FormatterResolverFactory formatterResolver = null;

            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in handlerMethods)
            {
                if (IsXmlReader<TComponent>(m))
                {
                    readHandler = (ReadHandlerDelegate<TComponent>) Delegate.CreateDelegate(typeof(ReadHandlerDelegate<TComponent>), null, m, false);
                }

                if (IsXmlWriter<TComponent>(m))
                {
                    writeHandler = (WriteHandlerDelegate<TComponent>)Delegate.CreateDelegate(typeof(WriteHandlerDelegate<TComponent>), null, m, false);
                }

                if (IsSurrogateProvider(m))
                {
                    formatterResolver = (FormatterResolverFactory)Delegate.CreateDelegate(typeof(FormatterResolverFactory), null, m, false);
                }
            }

            if (readHandler == null)
            {
                if (HasDataContract<TComponent>())
                {
                    readHandler = new DefaultDataContractReadHandler<TComponent>(objectResolver).Read;
                }
                else
                {
                    readHandler = new DefaultReadHandler<TComponent>().Read;
                }
            }

            if (writeHandler == null)
            {
                if (HasDataContract<TComponent>())
                {
                    writeHandler = new DefaultDataContractWriteHandler<TComponent>(objectResolver).Write;
                }
                else
                {
                    writeHandler = new DefaultWriteHandler<TComponent>().Write;
                }
            }

            r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, readHandler, attr.UsedAsTag).WithFormatterResolver(formatterResolver));
            r.Store(XmlWriteHandlerRegistration.Create(attr.ComponentTypeId, writeHandler, attr.UsedAsTag).WithFormatterResolver(formatterResolver));

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

        bool IsSurrogateProvider(MethodInfo methodInfo)
        {
            var paramType = typeof(IEntityKeyMapper);
            var returnType = typeof(ISerializationSurrogateProvider);
            return methodInfo.GetCustomAttribute<EntityXmlSurrogateProviderAttribute>() != null
                   && methodInfo.IsSameFunction(returnType, paramType);
        }

    }
}