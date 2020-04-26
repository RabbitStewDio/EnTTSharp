using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlEntityRegistrationHandler: EntityRegistrationHandlerBase
    {
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
            TranslateFunction<TComponent> translator = null;
            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in handlerMethods)
            {
                if (IsXmlReader<TComponent>(m))
                {
                    hasReadHandler = true;
                    var d = (ReadHandlerDelegate<TComponent>) Delegate.CreateDelegate(typeof(ReadHandlerDelegate<TComponent>), null, m, false);
                    r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, d));
                }

                if (IsXmlWriter<TComponent>(m))
                {
                    hasWriteHandler = true;
                    var d = (WriteHandlerDelegate<TComponent>)Delegate.CreateDelegate(typeof(WriteHandlerDelegate<TComponent>), null, m, false);
                    r.Store(XmlWriteHandlerRegistration.Create(d, attr.ComponentTypeId));
                }

                if (IsXmlReaderTranslator<TComponent>(m))
                {
                    translator = (TranslateFunction<TComponent>)Delegate.CreateDelegate(typeof(TranslateFunction<TComponent>), null, m, false);
                }
            }

            if (!hasReadHandler)
            {
                if (HasDataContract<TComponent>())
                {
                    ReadHandlerDelegate<TComponent> handler = new DefaultDataContractReadHandler<TComponent>(translator).Read;
                    r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, handler));
                }
                else
                {
                    ReadHandlerDelegate<TComponent> handler = new DefaultReadHandler<TComponent>(translator).Read;
                    r.Store(XmlReadHandlerRegistration.Create(attr.ComponentTypeId, handler));
                }
            }

            if (!hasWriteHandler)
            {
                if (HasDataContract<TComponent>())
                {
                    WriteHandlerDelegate<TComponent> handler = new DefaultDataContractWriteHandler<TComponent>().Write;
                    r.Store(XmlWriteHandlerRegistration.Create(handler, attr.ComponentTypeId));
                }
                else
                {
                    WriteHandlerDelegate<TComponent> handler = new DefaultWriteHandler<TComponent>().Write;
                    r.Store(XmlWriteHandlerRegistration.Create(handler, attr.ComponentTypeId));
                }
            }


        }

        bool HasDataContract<TComponent>()
        {
            var componentType = typeof(TComponent);
            return componentType.GetCustomAttribute<DataContractAttribute>() != null;
        }

        bool IsXmlReaderTranslator<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlReaderAttribute>() != null
                   && methodInfo.IsSameFunction(componentType, componentType, typeof(Func<EntityKey,EntityKey>));
        }

        bool IsXmlReader<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlReaderAttribute>() != null
                   && methodInfo.IsSameFunction(componentType, typeof(XmlReader), typeof(Func<EntityKey, EntityKey>));
        }

        bool IsXmlWriter<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityXmlWriterAttribute>() != null
                   && methodInfo.IsSameAction(typeof(XmlWriter), typeof(EntityKey), componentType);
        }

    }
}