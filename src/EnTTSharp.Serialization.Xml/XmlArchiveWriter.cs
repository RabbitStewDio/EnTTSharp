using System;
using System.Globalization;
using System.Xml;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Xml.Impl;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlArchiveWriter<TEntityKey> : IEntityArchiveWriter<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly XmlWriter writer;

        public XmlWriteHandlerRegistry Registry { get; }

        public XmlArchiveWriter(XmlWriteHandlerRegistry registry, XmlWriter writer)
        {
            this.Registry = registry;
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void WriteDefaultSnapshotDocumentHeader()
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("snapshot");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
        }

        public void WriteDefaultSnapshotDocumentFooter()
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }


        public void WriteStartDestroyed(in int entityCount)
        {
            writer.WriteStartElement(XmlTagNames.DestroyedEntities);
            writer.WriteAttributeString(XmlTagNames.CountAttribute, entityCount.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteDestroyed(in TEntityKey entity)
        {
            writer.WriteStartElement(XmlTagNames.DestroyedEntity);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            writer.WriteEndElement();
        }

        public void WriteEndDestroyed()
        {
            writer.WriteEndElement();
        }

        public void WriteStartEntity(in int entityCount)
        {
            writer.WriteStartElement(XmlTagNames.Entities);
            writer.WriteAttributeString(XmlTagNames.CountAttribute, entityCount.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteEntity(in TEntityKey entity)
        {
            writer.WriteStartElement(XmlTagNames.Entity);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            writer.WriteEndElement();
        }

        public void WriteEndEntity()
        {
            writer.WriteEndElement();
        }

        public void WriteStartComponent<TComponent>(in int entityCount)
        {
            writer.WriteStartElement(XmlTagNames.Components);
            writer.WriteAttributeString(XmlTagNames.CountAttribute, entityCount.ToString(CultureInfo.InvariantCulture));

            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteAttributeString("type", handler.TypeId);
        }

        public void WriteComponent<TComponent>(in TEntityKey entity, in TComponent c)
        {
            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Component);
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            handler.GetHandler<TComponent>().Invoke(writer, c);
            writer.WriteEndElement();
        }

        public void WriteEndComponent<TComponent>()
        {
            writer.WriteEndElement();
        }

        public void WriteMissingTag<TComponent>()
        {
            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Tag);
            writer.WriteAttributeString("type", handler.TypeId);
            writer.WriteAttributeString("missing", "true");
            writer.WriteEndElement();
        }

        public void WriteTag<TComponent>(in TEntityKey entity, in TComponent c)
        {
            var handler = Registry.QueryHandler<TComponent>();
            writer.WriteStartElement(XmlTagNames.Tag);
            writer.WriteAttributeString("type", handler.TypeId);
            writer.WriteAttributeString("missing", "false");
            writer.WriteAttributeString("entity-key", entity.Key.ToString("X"));
            writer.WriteAttributeString("entity-age", entity.Age.ToString("X"));
            handler.GetHandler<TComponent>().Invoke(writer, c);
            writer.WriteEndElement();
        }

        public void WriteEndOfFrame()
        {
            
        }

        public void FlushFrame()
        {
            this.writer.Flush();
        }
    }
}