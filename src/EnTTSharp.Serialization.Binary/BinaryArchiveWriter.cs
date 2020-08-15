using System;
using System.IO;
using EnttSharp.Entities;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Binary.Impl;
using MessagePack;

namespace EnTTSharp.Serialization.Binary
{
    public class BinaryArchiveWriter<TEntityKey>: IEntityArchiveWriter<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly BinaryWriteHandlerRegistry registry;
        readonly Stream writer;
        readonly MessagePackSerializerOptions options;

        public BinaryArchiveWriter(BinaryWriteHandlerRegistry registry, 
                                   Stream writer, MessagePackSerializerOptions options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (!writer.CanWrite)
            {
                throw new ArgumentException("The given stream must be writeable");
            }

            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.writer = writer;
            this.options = BinaryControlObjects.CreateOptions(options);
        }

        public void WriteStartEntity(in int entityCount)
        {
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Entities, options);
            MessagePackSerializer.Serialize(writer, entityCount, options);
        }

        public void WriteEntity(in TEntityKey entityKey)
        {
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
        }

        public void WriteEndEntity()
        {
        }

        public void WriteStartComponent<TComponent>(in int entityCount)
        {
            var handler = registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Component, options);
            MessagePackSerializer.Serialize(writer, new BinaryControlObjects.StartComponentRecord(entityCount, handler.TypeId));
        }

        public void WriteComponent<TComponent>(in TEntityKey entityKey, in TComponent c)
        {
            var handler = registry.QueryHandler<TComponent>();
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
            if (handler.TryGetPreProcessor<TComponent>(out var processor))
            {
                MessagePackSerializer.Serialize(writer, processor(c), options);
            }
            else
            {
                MessagePackSerializer.Serialize(writer, c, options);
            }
        }

        public void WriteEndComponent<TComponent>()
        {
        }

        public void WriteTag<TComponent>(in TEntityKey entityKey, in TComponent c)
        {
            var handler = registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Tag, options);
            MessagePackSerializer.Serialize(new BinaryControlObjects.StartTagRecord(true, handler.TypeId), options);
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
            if (handler.TryGetPreProcessor<TComponent>(out var processor))
            {
                MessagePackSerializer.Serialize(writer, processor(c), options);
            }
            else
            {
                MessagePackSerializer.Serialize(writer, c, options);
            }
        }

        public void WriteMissingTag<TComponent>()
        {
            var handler = registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Tag, options);
            MessagePackSerializer.Serialize(new BinaryControlObjects.StartTagRecord(false, handler.TypeId), options);
        }

        public void WriteStartDestroyed(in int entityCount)
        {
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.DestroyedEntities, options);
            MessagePackSerializer.Serialize(writer, entityCount, options);
        }

        public void WriteDestroyed(in TEntityKey entityKey)
        {
            var entityData = new EntityKeyData(entityKey.Age, entityKey.Key);
            MessagePackSerializer.Serialize(writer, entityData, options);
        }

        public void WriteEndDestroyed()
        {
        }

        public void WriteEndOfFrame()
        {
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.EndOfFrame, options);
        }

        public void FlushFrame()
        {
            writer.Flush();
        }
    }
}