using System;
using System.Reflection;
using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using MessagePack;
using MessagePack.Formatters;
using Serilog;

namespace EnTTSharp.Serialization.Binary.AutoRegistration
{
    public class BinaryEntityRegistrationHandler : EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<BinaryEntityRegistrationHandler>();

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            var componentType = typeof(TComponent);
            var msgPackAttr = componentType.GetCustomAttribute<MessagePackObjectAttribute>();
            var dataContractAttr = componentType.GetCustomAttribute<DataContractAttribute>();
            var binarySerializationAttribute = componentType.GetCustomAttribute<EntityBinarySerializationAttribute>();

            if (msgPackAttr == null && dataContractAttr == null && binarySerializationAttribute == null)
            {
                return;
            }

            var componentTypeId = binarySerializationAttribute?.ComponentTypeId ?? componentType.FullName;
            var usedAsTag = binarySerializationAttribute?.UsedAsTag ?? false;

            var handlerMethods = componentType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            BinaryPreProcessor<TComponent> preProcessor = null;
            BinaryPostProcessor<TComponent> postProcessor = null;
            FormatterResolverFactory resolverFactory = null;
            MessagePackFormatterFactory messageFormatterFactory = null;

            foreach (var m in handlerMethods)
            {
                if (IsResolverFactory(m))
                {
                    resolverFactory = (FormatterResolverFactory)Delegate.CreateDelegate(typeof(FormatterResolverFactory), null, m, false);
                }

                if (IsMessageFormatterFactory(m))
                {
                    messageFormatterFactory = (MessagePackFormatterFactory)Delegate.CreateDelegate(typeof(MessagePackFormatterFactory), null, m, false);
                }

                if (IsPostProcessor<TComponent>(m))
                {
                    postProcessor = (BinaryPostProcessor<TComponent>)Delegate.CreateDelegate(typeof(BinaryPostProcessor<TComponent>), null, m, false);
                }

                if (IsPreProcessor<TComponent>(m))
                {
                    preProcessor = (BinaryPreProcessor<TComponent>)Delegate.CreateDelegate(typeof(BinaryPreProcessor<TComponent>), null, m, false);
                }
            }

            r.Store(BinaryReadHandlerRegistration.Create(componentTypeId, usedAsTag, postProcessor)
                                                 .WithFormatterResolver(resolverFactory)
                                                 .WithMessagePackFormatter(messageFormatterFactory)
            );
            r.Store(BinaryWriteHandlerRegistration.Create(componentTypeId, usedAsTag, preProcessor)
                                                  .WithFormatterResolver(resolverFactory)
                                                  .WithMessagePackFormatter(messageFormatterFactory)
            );

            if (msgPackAttr == null)
            {
                Logger.Debug("Registered Binary DataContract Handling for {ComponentType}", componentType);
            }
            else
            {
                Logger.Debug("Registered Binary MessagePack Handling for {ComponentType}", componentType);
            }
        }

        bool IsMessageFormatterFactory(MethodInfo methodInfo)
        {
            var paramType = typeof(IEntityKeyMapper);
            var returnType = typeof(IMessagePackFormatter);
            return methodInfo.GetCustomAttribute<EntityBinaryFormatterAttribute>() != null
                   && methodInfo.IsSameFunction(returnType, paramType);
        }

        bool IsResolverFactory(MethodInfo methodInfo)
        {
            var paramType = typeof(IEntityKeyMapper);
            var returnType = typeof(IFormatterResolver);
            return methodInfo.GetCustomAttribute<EntityBinaryFormatterResolverAttribute>() != null
                   && methodInfo.IsSameFunction(returnType, paramType);
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