using System;
using System.Reflection;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlArchiveReader : IEntityArchiveReader
    {
        readonly XmlReadHandlerRegistry registry;
        readonly MethodInfo tagParserMethod;
        readonly MethodInfo componentParserMethod;

        public XmlArchiveReader(XmlReadHandlerRegistry registry)
        {
            this.registry = registry;

            var paramTypes = new []
            {
                typeof(XmlReader), typeof(ISnapshotLoader), typeof(EntityKey), typeof(XmlReadHandlerRegistration)
            };

            tagParserMethod = typeof(XmlArchiveReader).GetMethod(nameof(ParseTagInternal), 
                                                                 BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null) 
                              ?? throw new InvalidOperationException("Unable to find tag parsing wrapper method");
            componentParserMethod = typeof(XmlArchiveReader).GetMethod(nameof(ParseComponentInternal), 
                                                                 BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null) 
                              ?? throw new InvalidOperationException("Unable to find component parsing wrapper method");

        }

        protected EntityKey ReadEntity(XmlReader r)
        {
            var age = int.Parse(r.GetAttribute("entity-age") ?? throw new InvalidOperationException("Missing attribute 'age'"));
            var key = int.Parse(r.GetAttribute("entity-key") ?? throw new InvalidOperationException("Missing attribute 'key'"));
            var extraRaw = r.GetAttribute("entity-extra");
            var extra = 0u;
            if (!string.IsNullOrEmpty(extraRaw))
            {
                extra = uint.Parse(r.GetAttribute("extra") ?? "0");
            }

            var entity = new EntityKey((byte)age, key, extra);
            return entity;
        }

        bool HandleRootElement(XmlReader arg, ISnapshotLoader loader)
        {
            switch (arg.Name)
            {
                case XmlTagNames.Entity:
                {
                    var entity = ReadEntity(arg);
                    loader.OnEntity(entity);
                    return true;
                }
                case XmlTagNames.DestroyedEntity:
                {
                    var entity = ReadEntity(arg);
                    loader.OnDestroyedEntity(entity);
                    return true;
                }
                case XmlTagNames.Component:
                {
                    ReadComponent(arg, loader);
                    return true;
                }
                case XmlTagNames.Tag:
                {
                    ReadTag(arg, loader);
                    return true;
                }
            }
            return false;
        }

        void ReadTag(XmlReader reader, ISnapshotLoader loader)
        {
            var entity = ReadEntity(reader);
            var type = reader.GetAttribute("type") ?? throw new InvalidOperationException("Missing attribute 'type'");

            if (!registry.TryGetValue(type, out var handler))
            {
                throw new InvalidOperationException("No handler with type '" + type + "' defined.");
            }

            var _ = reader.Read();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        ParseTag(reader, loader, entity, handler);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        void ReadComponent(XmlReader reader, ISnapshotLoader loader)
        {
            var entity = ReadEntity(reader);
            var type = reader.GetAttribute("type") ?? throw new InvalidOperationException("Missing attribute 'type'");

            if (!registry.TryGetValue(type, out var handler))
            {
                throw new InvalidOperationException("No handler with type '" + type + "' defined.");
            }

            var _ = reader.Read();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        ParseComponent(reader, loader, entity, handler);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }

        void ParseTag(XmlReader reader, ISnapshotLoader loader, EntityKey entity, XmlReadHandlerRegistration handler)
        {
            var method = tagParserMethod.MakeGenericMethod(handler.TargetType);
            method.Invoke(this, new object[] {reader, loader, entity, handler});
        }

        void ParseTagInternal<TComponent>(XmlReader reader, ISnapshotLoader loader, EntityKey entity, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new InvalidOperationException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader, loader.Map);
            loader.OnTag(entity, in result);
        }

        void ParseComponent(XmlReader reader, ISnapshotLoader loader, EntityKey entity, XmlReadHandlerRegistration handler)
        {
            var method = componentParserMethod.MakeGenericMethod(handler.TargetType);
            method.Invoke(this, new object[] {reader, loader, entity, handler});
        }

        void ParseComponentInternal<TComponent>(XmlReader reader, ISnapshotLoader loader, EntityKey entity, XmlReadHandlerRegistration handler)
        {
            if (!handler.TryGetParser<TComponent>(out var parser))
            {
                throw new InvalidOperationException("Unable to resolve handler for registered type " + typeof(TComponent));
            }

            var result = parser(reader, loader.Map);
            loader.OnComponent(entity, in result);
        }

        public void Read(XmlReader reader, ISnapshotLoader loader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (!HandleRootElement(reader, loader))
                        {
                            reader.Skip();
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        return;
                    }
                }
            }
        }
    }
}