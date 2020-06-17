using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlArchiveReaderBackend
    {
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
                typeof(XmlReader), typeof(EntityKey), typeof(ISnapshotLoader), typeof(XmlReadHandlerRegistration)
            };

            tagParserMethod = typeof(XmlArchiveReaderBackend).GetMethod(nameof(ParseTagInternal),
                                                                        BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                        paramTypes, null)
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");

            componentParserMethod = typeof(XmlArchiveReaderBackend).GetMethod(nameof(ParseComponentInternal),
                                                                              BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                                                              paramTypes, null)
                                    ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");

            var missingTagParamTypes = new[]
            {
                typeof(ISnapshotLoader), typeof(XmlReadHandlerRegistration)
            };
            missingTagParserMethod = typeof(XmlArchiveReaderBackend).GetMethod(nameof(ParseMissingTagInternal),
                                                                               BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                                               missingTagParamTypes, null)
                                     ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");
        }

        public readonly XmlReadHandlerRegistry Registry;

        public EntityKey ReadEntity(XmlReader r)
        {
            return ReadEntity(r, XmlTagNames.Entity);
        }

        public EntityKey ReadDestroyedEntity(XmlReader r)
        {
            return ReadEntity(r, XmlTagNames.DestroyedEntity);
        }

        EntityKey ReadEntity(XmlReader r, string tag)
        {
            var age = int.Parse(r.GetAttribute("entity-age") ?? throw r.FromMissingAttribute(tag, "entity-age"));
            var key = int.Parse(r.GetAttribute("entity-key") ?? throw r.FromMissingAttribute(tag, "entity-key"));

            var extraRaw = r.GetAttribute("entity-extra");
            var extra = 0u;
            if (!string.IsNullOrEmpty(extraRaw))
            {
                extra = uint.Parse(r.GetAttribute("extra") ?? "0");
            }

            var entity = new EntityKey((byte)age, key, extra);
            return entity;
        }


        public void ReadTagTyped<TComponent>(XmlReader reader,
                                             Func<EntityKey, EntityKey> entityMapper,
                                             out EntityKey entity,
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

            entity = ReadEntity(reader, XmlTagNames.Tag);
            var _ = reader.Read();
            component = parser(reader, entityMapper);
        }

        public void ReadTag(XmlReader reader, ISnapshotLoader loader)
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

            var entity = ReadEntity(reader, XmlTagNames.Tag);

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
                                                   Func<EntityKey, EntityKey> entityMapper,
                                                   out EntityKey entity,
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

            entity = ReadEntity(reader, XmlTagNames.Component);
            var _ = reader.Read();
            component = parser(reader, entityMapper);
        }

        public void ReadComponent(XmlReader reader, ISnapshotLoader loader)
        {
            var entity = ReadEntity(reader, XmlTagNames.Component);
            var type = reader.GetAttribute("type") ?? throw reader.FromMissingAttribute(XmlTagNames.Component, "type");

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

        delegate void ParseAction(XmlReader reader, EntityKey entityRaw, ISnapshotLoader loader, XmlReadHandlerRegistration handler);

        void ParseTag(XmlReader reader, EntityKey entityRaw, ISnapshotLoader loader, XmlReadHandlerRegistration handler)
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

        void ParseTagInternal<TComponent>(XmlReader reader, EntityKey entityRaw, ISnapshotLoader loader, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader, loader.Map);
            loader.OnTag(loader.Map(entityRaw), in result);
        }

        void ParseComponent(XmlReader reader, EntityKey entityRaw, ISnapshotLoader loader, XmlReadHandlerRegistration handler)
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

        void ParseComponentInternal<TComponent>(XmlReader reader, EntityKey entityRaw, ISnapshotLoader loader, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new SnapshotIOException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader, loader.Map);
            loader.OnComponent(loader.Map(entityRaw), in result);
        }

        void ParseMissingTag(ISnapshotLoader loader, XmlReadHandlerRegistration handler)
        {
            if (cachedMissingTagDelegates.TryGetValue(handler.TargetType, out var actionRaw))
            {
                var action = (Action<ISnapshotLoader, XmlReadHandlerRegistration>)actionRaw;
                action(loader, handler);
                return;
            }

            var method = missingTagParserMethod.MakeGenericMethod(handler.TargetType);
            var actionDelegate = (Action<ISnapshotLoader, XmlReadHandlerRegistration>)
                Delegate.CreateDelegate(typeof(Action<ISnapshotLoader, XmlReadHandlerRegistration>), this, method);
            cachedMissingTagDelegates[handler.TargetType] = actionDelegate;
            actionDelegate(loader, handler);
        }

        void ParseMissingTagInternal<TComponent>(ISnapshotLoader loader, XmlReadHandlerRegistration handler)
        {
            loader.OnTagRemoved<TComponent>();
        }

    }
}