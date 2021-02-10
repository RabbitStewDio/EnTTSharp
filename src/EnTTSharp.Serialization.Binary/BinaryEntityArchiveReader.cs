using System.IO;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Binary.Impl;
using MessagePack;

namespace EnTTSharp.Serialization.Binary
{
    public class BinaryEntityArchiveReader<TEntityKey>: IEntityArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly MessagePackSerializerOptions options;
        readonly BinaryReaderBackend<TEntityKey> registry;
        readonly Stream reader;
        BinaryReadHandlerRegistration readHandlerRegistration;

        public BinaryEntityArchiveReader(BinaryReaderBackend<TEntityKey> registry, 
                                         Stream reader,
                                         MessagePackSerializerOptions optionsRaw = null)
        {
            this.registry = registry;
            this.reader = reader;
            this.options = optionsRaw;
        }

        public int ReadEntityCount()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.Entities)
            {
                throw new BinaryReaderException("Invalid stream state: Expected Entity-Start record");
            }

            return MessagePackSerializer.Deserialize<int>(reader, options);
        }

        public TEntityKey ReadEntity(IEntityKeyMapper entityMapper)
        {
            var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            return entityMapper.EntityKeyMapper<TEntityKey>(key);
        }

        public int ReadComponentCount<TComponent>()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.Component)
            {
                throw new BinaryReaderException("Invalid stream state: Expected Component-Start record");
            }

            var c = MessagePackSerializer.Deserialize<BinaryControlObjects.StartComponentRecord>(reader, options);
            if (!registry.Registry.TryGetValue(c.ComponentId, out readHandlerRegistration))
            {
                throw new BinaryReaderException($"Invalid stream state: No handler for component type {c.ComponentId}");
            }

            return c.ComponentCount;
        }

        public bool TryReadComponent<TComponent>(IEntityKeyMapper entityMapper, 
                                                 out TEntityKey key, out TComponent component)
        {

            var entityKey = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            component = MessagePackSerializer.Deserialize<TComponent>(reader, options);
            if (readHandlerRegistration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            key = entityMapper.EntityKeyMapper<TEntityKey>(entityKey);
            return true;
        }

        public bool ReadTagFlag<TComponent>()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.Tag)
            {
                throw new BinaryReaderException("Invalid stream state: Expected Tag-Start record");
            }

            var c = MessagePackSerializer.Deserialize<BinaryControlObjects.StartTagRecord>(reader, options);
            if (!registry.Registry.TryGetValue(c.ComponentId, out readHandlerRegistration))
            {
                throw new BinaryReaderException($"Invalid stream state: No handler for component type {c.ComponentId}");
            }

            return c.ComponentExists;
        }

        public bool TryReadTag<TComponent>(IEntityKeyMapper entityMapper, out TEntityKey key, out TComponent component)
        {
            var entityKey = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            component = MessagePackSerializer.Deserialize<TComponent>(reader, options);
            if (readHandlerRegistration.TryGetPostProcessor<TComponent>(out var pp))
            {
                component = pp(in component);
            }

            key = entityMapper.EntityKeyMapper<TEntityKey>(entityKey);
            return true;
        }

        public int ReadDestroyedCount()
        {
            var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
            if (recordType != BinaryControlObjects.BinaryStreamState.DestroyedEntities)
            {
                throw new BinaryReaderException("Invalid stream state: Expected DestroyedEntities-Start record");
            }

            return MessagePackSerializer.Deserialize<int>(reader, options);
        }

        public TEntityKey ReadDestroyed(IEntityKeyMapper entityMapper)
        {
            var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
            return entityMapper.EntityKeyMapper<TEntityKey>(key);
        }
    }
}