using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EnttSharp.Entities;
using MessagePack;

namespace EnTTSharp.Serialization.BinaryPack
{
    public class BinaryReaderBackend
    {
        delegate void ParseFunctionDelegate(Stream s, EntityKey k, 
                                            ISnapshotLoader l, BinaryReadHandlerRegistration r, 
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
                typeof(EntityKey), 
                typeof(ISnapshotLoader), 
                typeof(BinaryReadHandlerRegistration),
                typeof(MessagePackSerializerOptions)
            };

            tagParserMethod = typeof(BinaryReaderBackend).GetMethod(nameof(ParseTagInternal),
                                                                    BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                    paramTypes, null)
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");

            componentParserMethod = typeof(BinaryReaderBackend).GetMethod(nameof(ParseComponentInternal),
                                                                          BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null)
                                    ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");

            var missingTagParamTypes = new[]
            {
                typeof(ISnapshotLoader),
                typeof(BinaryReadHandlerRegistration)
            };

            missingTagParserMethod = typeof(BinaryReaderBackend).GetMethod(nameof(ParseMissingTagInternal),
                                                                           BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                                           missingTagParamTypes, null)
                                     ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");
        }

        public void ReadTag(Stream stream, ISnapshotLoader loader, 
                            MessagePackSerializerOptions options)
        {
            var startTagRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartTagRecord>(stream, options);
            if (!Registry.TryGetValue(startTagRecord.ComponentId, out var handler))
            {
                throw new BinaryReaderException($"Corrupted stream state: No handler for component type {startTagRecord.ComponentId}");
            }

            if (startTagRecord.ComponentExists)
            {
                var entityKey = MessagePackSerializer.Deserialize<EntityKey>(stream, options);
                ParseTag(stream, entityKey, loader, handler, options);
            }
            else
            {
                ParseMissingTag(loader, handler);
            }
        }

        void ParseMissingTag(ISnapshotLoader loader, BinaryReadHandlerRegistration handler)
        {
            if (cachedMissingTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var parseAction = (Action<ISnapshotLoader, BinaryReadHandlerRegistration>)actionRaw;
                parseAction(loader, handler);
                return;
            }

            var method = missingTagParserMethod.MakeGenericMethod(handler.TargetType);
            var newParseAction = (Action<ISnapshotLoader, BinaryReadHandlerRegistration>)
                Delegate.CreateDelegate(typeof(Action<ISnapshotLoader, BinaryReadHandlerRegistration>), this, method);
            cachedMissingTagDelegates[handler.TargetType] = newParseAction;
            newParseAction(loader, handler);

        }

        void ParseMissingTagInternal<TComponent>(ISnapshotLoader loader, BinaryReadHandlerRegistration reg)
        {
            loader.OnTagRemoved<TComponent>();
        }

        void ParseTag(Stream reader, 
                      EntityKey entityRaw, 
                      ISnapshotLoader loader, 
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
                                          EntityKey entity, 
                                          ISnapshotLoader loader, 
                                          BinaryReadHandlerRegistration registration,
                                          MessagePackSerializerOptions options)
        {
            var component = MessagePackSerializer.Deserialize<TComponent>(stream, options);
            if (registration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            loader.OnTag(loader.Map(entity), component);
        }

        public void ReadComponent(Stream stream, ISnapshotLoader loader, BinaryReadHandlerRegistration registration,
                                  MessagePackSerializerOptions options)
        {
            var entityKey = MessagePackSerializer.Deserialize<EntityKey>(stream, options);
            ParseComponent(stream, entityKey, loader, registration, options);
        }

        void ParseComponent(Stream reader, EntityKey entityRaw,
                            ISnapshotLoader loader, BinaryReadHandlerRegistration handler,
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

        void ParseComponentInternal<TComponent>(Stream stream, EntityKey entity, 
                                                ISnapshotLoader loader, BinaryReadHandlerRegistration registration,
                                                MessagePackSerializerOptions options)
        {
            var component = MessagePackSerializer.Deserialize<TComponent>(stream, options);
            if (registration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            loader.OnComponent(loader.Map(entity), component);
        }
    }
}