using System;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlArchiveWriter : IEntityArchiveWriter
    {
        readonly XmlWriteHandlerRegistry registry;
        readonly XmlWriter writer;

        public XmlArchiveWriter(XmlWriteHandlerRegistry registry, XmlWriter writer)
        {
            this.registry = registry;
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void WriteDestroyed(in EntityKey entity)
        {
            writer.WriteStartElement(XmlTagNames.DestroyedEntity);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            if (entity.Extra != 0)
            {
                writer.WriteAttributeString("entity-extra", entity.Extra.ToString("X"));
            }
            writer.WriteEndElement();
        }

        public void WriteEntity(in EntityKey entity)
        {
            writer.WriteStartElement(XmlTagNames.Entity);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            if (entity.Extra != 0)
            {
                writer.WriteAttributeString("entity-extra", entity.Extra.ToString("X"));
            }
            writer.WriteEndElement();
        }

        public void WriteComponent<TComponent>(in EntityKey entity, in TComponent c)
        {
            var handler = registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Component);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            if (entity.Extra != 0)
            {
                writer.WriteAttributeString("entity-extra", entity.Extra.ToString("X"));
            }
            writer.WriteAttributeString("type", handler.TypeId);
            handler.GetHandler<TComponent>().Invoke(writer, entity, c);
            writer.WriteEndElement();
        }

        public void WriteTag<TComponent>(in EntityKey entity, in TComponent c)
        {
            var handler = registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Tag);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            if (entity.Extra != 0)
            {
                writer.WriteAttributeString("entity-extra", entity.Extra.ToString("X"));
            }
            writer.WriteAttributeString("type", handler.TypeId);
            handler.GetHandler<TComponent>().Invoke(writer, entity, c);
            writer.WriteEndElement();
        }

        public void FlushFrame()
        {
            this.writer.Flush();
        }
    }
}