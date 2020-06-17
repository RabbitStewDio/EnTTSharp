using System;
using System.Reflection;
using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using MessagePack;
using Serilog;

namespace EnTTSharp.Serialization.BinaryPack.AutoRegistration
{
    public class BinaryEntityRegistrationHandler: EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<BinaryEntityRegistrationHandler>();

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            var componentType = typeof(TComponent);
            var msgPackAttr = componentType.GetCustomAttribute<MessagePackObjectAttribute>();
            var dataContractAttr = componentType.GetCustomAttribute<DataContractAttribute>();
            if (msgPackAttr == null && dataContractAttr == null)
            {
                return;
            }

            var attr2 = componentType.GetCustomAttribute<EntityBinarySerializationAttribute>();
            var componentTypeId = attr2?.ComponentTypeId ?? componentType.FullName;
            var usedAsTag = attr2?.UsedAsTag ?? false;

            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            var registeredWriteHandler = false;
            var registeredReadHandler = false;
            foreach (var m in handlerMethods)
            {
                if (IsPostProcessor<TComponent>(m))
                {
                    registeredReadHandler = true;
                    var d = (BinaryPostProcessor<TComponent>)Delegate.CreateDelegate(typeof(BinaryPostProcessor<TComponent>), null, m, false);
                    r.Store(BinaryReadHandlerRegistration.Create(componentTypeId, usedAsTag, d));
                }

                if (IsPreProcessor<TComponent>(m))
                {
                    registeredWriteHandler = true;
                    var d = (BinaryPostProcessor<TComponent>)Delegate.CreateDelegate(typeof(BinaryPostProcessor<TComponent>), null, m, false);
                    r.Store(BinaryWriteHandlerRegistration.Create(componentTypeId, usedAsTag, d));
                }
            }

            if (!registeredReadHandler)
            {
                r.Store(BinaryReadHandlerRegistration.Create<TComponent>(componentTypeId, usedAsTag));
            }
            if (!registeredWriteHandler)
            {
                r.Store(BinaryWriteHandlerRegistration.Create<TComponent>(componentTypeId, usedAsTag));
            }

            if (msgPackAttr == null)
            {
                Logger.Debug("Registered Binary DataContract Handling for {ComponentType}", componentType);
            }
            else
            {
                Logger.Debug("Registered Binary MessagePack Handling for {ComponentType}", componentType);

            }
        }

        bool IsPreProcessor<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityBinaryPreProcessorAttribute>() != null
                   && methodInfo.IsSameFunction(componentType, componentType);
        }

        bool IsPostProcessor<TComponent>(MethodInfo methodInfo)
        {
            var componentType = typeof(TComponent);
            return methodInfo.GetCustomAttribute<EntityBinaryPostProcessorAttribute>() != null
                   && methodInfo.IsSameFunction(componentType, componentType);
        }

    }
}