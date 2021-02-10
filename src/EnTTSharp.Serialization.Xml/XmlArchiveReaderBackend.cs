using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Xml.Impl;
using Serilog;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlArchiveReaderBackend<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly ILogger Logger = LogHelper.ForContext<XmlArchiveReaderBackend<TEntityKey>>();
        
        readonly MethodInfo tagParserMethod;
        readonly MethodInfo missingTagParserMethod;
        readonly MethodInfo componentParserMethod;
        readonly Dictionary<Type, object> cachedComponentDelegates;
        readonly Dictionary<Type, object> cachedTagDelegates;
        readonly Dictionary<Type, object> cachedMissingTagDelegates;

        public XmlArchiveReaderBackend(XmlReadHandlerRegistry registry)
        {
            this.Registry = registry;
            this.cachedTagDelegates = new Dictionary<Type, object>();
            this.cachedComponentDelegates = new Dictionary<Type, object>();
            this.cachedMissingTagDelegates = new Dictionary<Type, object>();

            var paramTypes = new[]
            {
                typeof(XmlReader), typeof(TEntityKey), typeof(ISnapshotLoader<TEntityKey>), typeof(XmlReadHandlerRegistration)
            };

            tagParserMethod = typeof(XmlArchiveReaderBackend<TEntityKey>).GetMethod(nameof(ParseTagInternal),
                                                                                    BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                                    paramTypes, null)
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");

            componentParserMethod = typeof(XmlArchiveReaderBackend<TEntityKey>).GetMethod(nameof(ParseComponentInternal),
                                                                                          BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                                          paramTypes, null)
                                    ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");

            var missingTagParamTypes = new[]
            {
                typeof(ISnapshotLoader<TEntityKey>)
            };
            missingTagParserMethod = typeof(XmlArchiveReaderBackend<TEntityKey>).GetMethod(nameof(ParseMissingTagInternal),
                                                                                           BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                                                           missingTagParamTypes, null)
                                     ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");
        }

        public readonly XmlReadHandlerRegistry Registry;

        public TEntityKey ReadEntity(XmlReader r,
                                     IEntityKeyMapper entityMapper)
        {
            return entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(r, XmlTagNames.Entity));
        }

        public TEntityKey ReadDestroyedEntity(XmlReader r,
                                              IEntityKeyMapper entityMapper)
        {
            return entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(r, XmlTagNames.DestroyedEntity));
        }

        EntityKeyData ReadEntityData(XmlReader r, string tag)
        {
            var age = byte.Parse(r.GetAttribute("entity-age") ?? throw r.FromMissingAttribute(tag, "entity-age"));
            var key = int.Parse(r.GetAttribute("entity-key") ?? throw r.FromMissingAttribute(tag, "entity-key"));
            Logger.Verbose("Reading Entity Data: Age: {Age}, Key: {Key}", age, key);
            return new EntityKeyData(age, key);
        }


        public void ReadTagTyped<TComponent>(XmlReader reader,
                                             IEntityKeyMapper entityMapper,
                                             out TEntityKey entity,
                                             out TComponent component)
        {
            if (!Registry.TryGetValue(typeof(TComponent), out var handler))
            {
                throw new InvalidOperationException("No handler with type '" + typeof(TComponent) + "' defined.");
            }

            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new InvalidOperationException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            entity = entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(reader, XmlTagNames.Tag));
            var _ = reader.Read();
            Logger.Verbose("Reading Component for {Entity}", entity);
            component = parser(reader);
        }

        public void ReadTag(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            var type = reader.GetAttribute("type") ?? throw reader.FromMissingAttribute(XmlTagNames.Tag, "type");
            if (!Registry.TryGetValue(type, out var handler))
            {
                throw new SnapshotIOException("No handler with type '" + type + "' defined.");
            }

            var missing = reader.GetAttribute("missing");
            if (string.Equals(missing, "true", StringComparison.InvariantCultureIgnoreCase))
            {
                ParseMissingTag(loader, handler);
                reader.Skip();
                return;
            }

            var entity = loader.Map(ReadEntityData(reader, XmlTagNames.Tag));

            var _ = reader.Read();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    ParseTag(reader, entity, loader, handler);
                }
            }
        }

        public void ReadComponentTyped<TComponent>(XmlReader reader,
                                                   IEntityKeyMapper entityMapper,
                                                   out TEntityKey entity,
                                                   out TComponent component)
        {
            if (!Registry.TryGetValue(typeof(TComponent), out var handler))
            {
                throw new SnapshotIOException("No handler with type '" + typeof(TComponent) + "' defined.");
            }

            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            entity = entityMapper.EntityKeyMapper<TEntityKey>(ReadEntityData(reader, XmlTagNames.Component));
            var _ = reader.Read();
            component = parser(reader);
        }

        public void ReadComponent(XmlReader reader, ISnapshotLoader<TEntityKey> loader, string type)
        {
            var entity = loader.Map(ReadEntityData(reader, XmlTagNames.Component));

            if (!Registry.TryGetValue(type, out var handler))
            {
                throw new SnapshotIOException($"No parse handler defined for component-type '{type}'");
            }

            var _ = reader.Read();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        ParseComponent(reader, entity, loader, handler);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        delegate void ParseAction(XmlReader reader, 
                                  TEntityKey entityRaw, 
                                  ISnapshotLoader<TEntityKey> loader, 
                                  XmlReadHandlerRegistration handler);

        void ParseTag(XmlReader reader, TEntityKey entityRaw, ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (cachedTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (ParseAction)actionRaw;
                action(reader, entityRaw, loader, handler);
                return;
            }

            var method = tagParserMethod.MakeGenericMethod(handler.TargetType);
            var actionDelegate = (ParseAction)Delegate.CreateDelegate(typeof(ParseAction), this, method);
            cachedTagDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(reader, entityRaw, loader, handler);
        }

        void ParseTagInternal<TComponent>(XmlReader reader, TEntityKey entityRaw, 
                                          ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader);
            loader.OnTag(entityRaw, in result);
        }

        void ParseComponent(XmlReader reader, TEntityKey entityRaw, ISnapshotLoader<TEntityKey> loader, 
                            XmlReadHandlerRegistration handler)
        {
            if (cachedComponentDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (ParseAction)actionRaw;
                action(reader, entityRaw, loader, handler);
                return;
            }

            var method = componentParserMethod.MakeGenericMethod(handler.TargetType);
            var actionDelegate = (ParseAction)Delegate.CreateDelegate(typeof(ParseAction), this, method);
            cachedComponentDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(reader, entityRaw, loader, handler);
        }

        void ParseComponentInternal<TComponent>(XmlReader reader, TEntityKey entityRaw, 
                                                ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader);
            loader.OnComponent(entityRaw, in result);
        }

        void ParseMissingTag(ISnapshotLoader<TEntityKey> loader, XmlReadHandlerRegistration handler)
        {
            if (cachedMissingTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (Action<ISnapshotLoader<TEntityKey>>)actionRaw;
                action(loader);
                return;
            }

            var method = missingTagParserMethod.MakeGenericMethod(handler.TargetType);
            
            var actionDelegate = (Action<ISnapshotLoader<TEntityKey>>)Delegate.CreateDelegate(typeof(Action<ISnapshotLoader<TEntityKey>>), this, method);
            cachedMissingTagDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(loader);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        void ParseMissingTagInternal<TComponent>(ISnapshotLoader<TEntityKey> loader)
        {
            loader.OnTagRemoved<TComponent>();
        }

    }
}