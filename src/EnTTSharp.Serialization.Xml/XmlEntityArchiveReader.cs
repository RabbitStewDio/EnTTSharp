using System;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlEntityArchiveReader : IEntityArchiveReader
    {
        readonly XmlArchiveReaderBackend readerConfiguration;
        readonly XmlReader reader;

        public XmlEntityArchiveReader(XmlReadHandlerRegistry readerConfiguration, XmlReader reader): 
            this(new XmlArchiveReaderBackend(readerConfiguration), reader)
        {}

        public XmlEntityArchiveReader(XmlArchiveReaderBackend readerConfiguration, XmlReader reader)
        {
            this.readerConfiguration = readerConfiguration ?? throw new ArgumentNullException(nameof(readerConfiguration));
            this.reader = reader;
        }

        public XmlReadHandlerRegistry Registry
        {
            get { return readerConfiguration.Registry; }
        }

        public int ReadEntityCount()
        {
            reader.AdvanceToElement(XmlTagNames.Entities);
            if (reader.TryGetAttributeInt(XmlTagNames.CountAttribute, out var value))
            {
                return value;
            }
            throw new SnapshotIOException("Unable to parse 'count' attribute on element 'Entities'");
        }

        public EntityKey ReadEntity(Func<EntityKey, EntityKey> entityMapper)
        {
            reader.AdvanceToElement(XmlTagNames.Entity);
            return entityMapper(readerConfiguration.ReadEntity(reader));
        }

        public int ReadComponentCount<TComponent>()
        {
            reader.AdvanceToElement(XmlTagNames.Components);
            if (reader.TryGetAttributeInt(XmlTagNames.CountAttribute, out var value))
            {
                return value;
            }
            throw new SnapshotIOException("Unable to parse 'count' attribute on element 'Components'");
        }

        public bool TryReadComponent<TComponent>(Func<EntityKey, EntityKey> entityMapper, 
                                                 out EntityKey key, out TComponent component)
        {
            reader.AdvanceToElement(XmlTagNames.Component);
            readerConfiguration.ReadComponentTyped(reader, entityMapper, out key, out component);
            return true;
        }

        public bool ReadTagFlag<TComponent>()
        {
            reader.AdvanceToElement(XmlTagNames.Tag);
            if (reader.TryGetAttributeBool("missing", out var value))
            {
                // missing=false indicates that the tag is declared.
                return value == false;
            }

            return false;
        }

        public bool TryReadTag<TComponent>(Func<EntityKey, EntityKey> entityMapper,
                                           out EntityKey entityKey, out TComponent component)
        {
            readerConfiguration.ReadTagTyped(reader, entityMapper, out entityKey, out component);
            return true;
        }

        public int ReadDestroyedCount()
        {
            reader.AdvanceToElement(XmlTagNames.DestroyedEntities);
            if (reader.TryGetAttributeInt(XmlTagNames.CountAttribute, out var value))
            {
                return value;
            }
            throw new SnapshotIOException("Unable to parse 'count' attribute on element 'DestroyedEntities'");
        }

        public EntityKey ReadDestroyed(Func<EntityKey, EntityKey> entityMapper)
        {
            return entityMapper(readerConfiguration.ReadDestroyedEntity(reader));
        }
    }
}