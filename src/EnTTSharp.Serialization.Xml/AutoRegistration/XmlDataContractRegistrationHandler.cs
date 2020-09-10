using System;
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

            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            FormatterResolverFactory<TEntityKey> formatterResolver = null;
            foreach (var m in handlerMethods)
            {
                if (IsSurrogateProvider(m))
                {
                    formatterResolver = (FormatterResolverFactory<TEntityKey>)Delegate.CreateDelegate(typeof(FormatterResolverFactory<TEntityKey>), null, m, false);
                }
            }


            ReadHandlerDelegate<TComponent> readHandler = new DefaultDataContractReadHandler<TComponent>(objectResolver).Read;
            r.Store(XmlReadHandlerRegistration.Create(null, readHandler, false).WithFormatterResolver(formatterResolver));

            WriteHandlerDelegate<TComponent> writeHandler = new DefaultDataContractWriteHandler<TComponent>(objectResolver).Write;
            r.Store(XmlWriteHandlerRegistration.Create(null, writeHandler, false).WithFormatterResolver(formatterResolver));

            Logger.Debug("Registered Xml DataContract Handling for {ComponentType}", componentType);
        }

        bool IsSurrogateProvider(MethodInfo methodInfo)
        {
            var paramType = typeof(EntityKeyMapper<TEntityKey>);
            var returnType = typeof(ISerializationSurrogateProvider);
            return methodInfo.GetCustomAttribute<EntityXmlSurrogateProviderAttribute>() != null
                   && methodInfo.IsSameFunction(returnType, paramType);
        }


    }
}