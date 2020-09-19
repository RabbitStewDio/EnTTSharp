using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Binary.Impl;
using MessagePack;

namespace EnTTSharp.Serialization.Binary
{
    public class BinaryReaderBackend<TEntityKey> where TEntityKey : IEntityKey
    {
        delegate void ParseFunctionDelegate(Stream s, TEntityKey k, 
                                            ISnapshotLoader<TEntityKey> l, 
                                            BinaryReadHandlerRegistration r, 
                                            MessagePackSerializerOptions options);

        public readonly BinaryReadHandlerRegistry Registry;
        readonly MethodInfo missingTagParserMethod;
        readonly MethodInfo tagParserMethod;
        readonly MethodInfo componentParserMethod;
        readonly Dictionary<Type, object> cachedComponentDelegates;
        readonly Dictionary<Type, object> cachedTagDelegates;
        readonly Dictionary<Type, object> cachedMissingTagDelegates;

        public BinaryReaderBackend(BinaryReadHandlerRegistry registry)
        {
            this.Registry = registry;
            this.cachedTagDelegates = new Dictionary<Type, object>();
            this.cachedComponentDelegates = new Dictionary<Type, object>();
            this.cachedMissingTagDelegates = new Dictionary<Type, object>();


            var paramTypes = new[]
            {
                typeof(Stream), 
                typeof(TEntityKey), 
                typeof(ISnapshotLoader<TEntityKey>), 
                typeof(BinaryReadHandlerRegistration),
                typeof(MessagePackSerializerOptions)
            };

            tagParserMethod = typeof(BinaryReaderBackend<TEntityKey>).GetMethod(nameof(ParseTagInternal),
                                                                                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                                paramTypes, null)
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");

            componentParserMethod = typeof(BinaryReaderBackend<TEntityKey>).GetMethod(nameof(ParseComponentInternal),
                                                                                      BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null)
                                    ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");

            var missingTagParamTypes = new[]
            {
                typeof(ISnapshotLoader<TEntityKey>)
            };

            missingTagParserMethod = typeof(BinaryReaderBackend<TEntityKey>).GetMethod(nameof(ParseMissingTagInternal),
                                                                                       BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                                                       missingTagParamTypes, null)
                                     ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");
        }

        public void ReadTag(Stream stream, ISnapshotLoader<TEntityKey> loader, 
                            MessagePackSerializerOptions options)
        {
            var startTagRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartTagRecord>(stream, options);
            if (!Registry.TryGetValue(startTagRecord.ComponentId, out var handler))
            {
                throw new BinaryReaderException($"Corrupted stream state: No handler for component type {startTagRecord.ComponentId}");
            }

            if (startTagRecord.ComponentExists)
            {
                var entityKey = MessagePackSerializer.Deserialize<EntityKeyData>(stream, options);
                ParseTag(stream, loader.Map(entityKey), loader, handler, options);
            }
            else
            {
                ParseMissingTag(loader, handler);
            }
        }

        void ParseMissingTag(ISnapshotLoader<TEntityKey> loader, BinaryReadHandlerRegistration handler)
        {
            if (cachedMissingTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (Action<ISnapshotLoader<TEntityKey>, BinaryReadHandlerRegistration>)actionRaw;
                parseAction(loader, handler);
                return;
            }

            var method = missingTagParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (Action<ISnapshotLoader<TEntityKey>, BinaryReadHandlerRegistration>)
                Delegate.CreateDelegate(typeof(Action<ISnapshotLoader<TEntityKey>, BinaryReadHandlerRegistration>), this, method);
            cachedMissingTagDelegates[handler.TargetType] = newParseAction;
            newParseAction(loader, handler);

        }

        void ParseMissingTagInternal<TComponent>(ISnapshotLoader<TEntityKey> loader)
        {
            loader.OnTagRemoved<TComponent>();
        }

        void ParseTag(Stream reader, 
                      TEntityKey entityRaw, 
                      ISnapshotLoader<TEntityKey> loader, 
                      BinaryReadHandlerRegistration handler,
                      MessagePackSerializerOptions options)
        {
            if (cachedTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (ParseFunctionDelegate)actionRaw;
                parseAction(reader, entityRaw, loader, handler, options);
                return;
            }

            var method = tagParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (ParseFunctionDelegate)Delegate.CreateDelegate(typeof(ParseFunctionDelegate), this, method);
            cachedTagDelegates[handler.TargetType] = newParseAction;
            newParseAction(reader, entityRaw, loader, handler, options);
        }

        void ParseTagInternal<TComponent>(Stream stream, 
                                          TEntityKey entity, 
                                          ISnapshotLoader<TEntityKey> loader, 
                                          BinaryReadHandlerRegistration registration,
                                          MessagePackSerializerOptions options)
        {
            var component = MessagePackSerializer.Deserialize<TComponent>(stream, options);
            if (registration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            loader.OnTag(entity, component);
        }

        public void ReadComponent(Stream stream, 
                                  ISnapshotLoader<TEntityKey> loader, 
                                  BinaryReadHandlerRegistration registration,
                                  MessagePackSerializerOptions options)
        {
            var entityKeyData = MessagePackSerializer.Deserialize<EntityKeyData>(stream, options);
            var entityKey = loader.Map(entityKeyData);
            ParseComponent(stream, entityKey, loader, registration, options);
        }

        void ParseComponent(Stream reader, TEntityKey entityRaw,
                            ISnapshotLoader<TEntityKey> loader, BinaryReadHandlerRegistration handler,
                            MessagePackSerializerOptions options)
        {
            if (cachedComponentDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (ParseFunctionDelegate)actionRaw;
                parseAction(reader, entityRaw, loader, handler, options);
                return;
            }

            var method = componentParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (ParseFunctionDelegate)Delegate.CreateDelegate(typeof(ParseFunctionDelegate), this, method);
            cachedComponentDelegates[handler.TargetType] = newParseAction;
            newParseAction(reader, entityRaw, loader, handler, options);
        }

        void ParseComponentInternal<TComponent>(Stream stream, 
                                                TEntityKey entity, 
                                                ISnapshotLoader<TEntityKey> loader, 
                                                BinaryReadHandlerRegistration registration,
                                                MessagePackSerializerOptions options)
        {
            var component = MessagePackSerializer.Deserialize<TComponent>(stream, options);
            if (registration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            loader.OnComponent(entity, component);
        }
    }
}