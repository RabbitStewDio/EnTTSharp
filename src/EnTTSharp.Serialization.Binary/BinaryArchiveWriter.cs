using System;
using System.IO;
using EnttSharp.Entities;
using MessagePack;

namespace EnTTSharp.Serialization.BinaryPack
{
    public class BinaryArchiveWriter: IEntityArchiveWriter
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

        public void WriteEntity(in EntityKey entityKey)
        {
            MessagePackSerializer.Serialize(writer, entityKey, options);
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

        public void WriteComponent<TComponent>(in EntityKey entityKey, in TComponent c)
        {
            var handler = registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, entityKey, options);
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

        public void WriteTag<TComponent>(in EntityKey entityKey, in TComponent c)
        {
            var handler = registry.QueryHandler<TComponent>();
            MessagePackSerializer.Serialize(writer, BinaryControlObjects.BinaryStreamState.Tag, options);
            MessagePackSerializer.Serialize(new BinaryControlObjects.StartTagRecord(true, handler.TypeId), options);
            MessagePackSerializer.Serialize(writer, entityKey, options);
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

        public void WriteDestroyed(in EntityKey entityKey)
        {
            MessagePackSerializer.Serialize(writer, entityKey, options);
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