using System;
using System.Xml;
using EnttSharp.Entities;
using EnTTSharp.Serialization.Xml.Impl;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlEntityArchiveReader<TEntityKey> : IEntityArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly XmlArchiveReaderBackend<TEntityKey> readerConfiguration;
        readonly XmlReader reader;

        public XmlEntityArchiveReader(XmlReadHandlerRegistry readerConfiguration, XmlReader reader): 
            this(new XmlArchiveReaderBackend<TEntityKey>(readerConfiguration), reader)
        {}

        public XmlEntityArchiveReader(XmlArchiveReaderBackend<TEntityKey> readerConfiguration, XmlReader reader)
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

        public TEntityKey ReadEntity(Func<EntityKeyData, TEntityKey> entityMapper)
        {
            reader.AdvanceToElement(XmlTagNames.Entity);
            return readerConfiguration.ReadEntity(reader, entityMapper);
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

        public bool TryReadComponent<TComponent>(Func<EntityKeyData, TEntityKey> entityMapper, 
                                                 out TEntityKey key, out TComponent component)
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

        public bool TryReadTag<TComponent>(Func<EntityKeyData, TEntityKey> entityMapper,
                                           out TEntityKey entityKey, out TComponent component)
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

        public TEntityKey ReadDestroyed(Func<EntityKeyData, TEntityKey> entityMapper)
        {
            return readerConfiguration.ReadDestroyedEntity(reader, entityMapper);
        }
    }
}