using System;
using System.Reflection;
using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using MessagePack;
using Serilog;

namespace EnTTSharp.Serialization.Binary.AutoRegistration
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
            BinaryPreProcessor<TComponent> preProcessor = null;
            BinaryPostProcessor<TComponent> postProcessor = null;
            foreach (var m in handlerMethods)
            {
                if (IsPostProcessor<TComponent>(m))
                {
                    postProcessor = (BinaryPostProcessor<TComponent>)Delegate.CreateDelegate(typeof(BinaryPostProcessor<TComponent>), null, m, false);
                }

                if (IsPreProcessor<TComponent>(m))
                {
                    preProcessor = (BinaryPreProcessor<TComponent>)Delegate.CreateDelegate(typeof(BinaryPreProcessor<TComponent>), null, m, false);
                }
            }

            r.Store(BinaryReadHandlerRegistration.Create(componentTypeId, usedAsTag, postProcessor));
            r.Store(BinaryWriteHandlerRegistration.Create(componentTypeId, usedAsTag, preProcessor));

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