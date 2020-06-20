using System;
using System.IO;
using EnttSharp.Entities;
using MessagePack;

namespace EnTTSharp.Serialization.BinaryPack
{
    public class BinaryBulkArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly MessagePackSerializerOptions options;
        readonly BinaryReaderBackend<TEntityKey> registry;

        public BinaryBulkArchiveReader(BinaryReaderBackend<TEntityKey> registry, MessagePackSerializerOptions optionsRaw = null)
        {
            this.registry = registry;
            this.options = BinaryControlObjects.CreateOptions(optionsRaw);
        }

        public void ReadAll(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            for (;;)
            {
                var recordType = MessagePackSerializer.Deserialize<BinaryControlObjects.BinaryStreamState>(reader, options);
                switch (recordType)
                {
                    case BinaryControlObjects.BinaryStreamState.DestroyedEntities:
                        ReadDestroyedEntities(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.Entities:
                        ReadEntities(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.Component:
                        ReadComponents(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.Tag:
                        ReadTag(reader, loader);
                        break;
                    case BinaryControlObjects.BinaryStreamState.EndOfFrame:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void ReadTag(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            registry.ReadTag(reader, loader, options);
        }

        void ReadComponents(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            var startRecord = MessagePackSerializer.Deserialize<BinaryControlObjects.StartComponentRecord>(reader, options);
            if (!registry.Registry.TryGetValue(startRecord.ComponentId, out var handler))
            {
                throw new BinaryReaderException($"Corrupted stream state: No handler for component type {startRecord.ComponentId}");
            }

            for (int c = 0; c < startRecord.ComponentCount; c += 1)
            {
                registry.ReadComponent(reader, loader, handler, options);
            }
        }

        void ReadDestroyedEntities(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            var entityCount = MessagePackSerializer.Deserialize<int>(reader, options);
            for (int e = 0; e < entityCount; e += 1)
            {
                var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
                loader.OnDestroyedEntity(loader.Map(key));
            }
        }

        void ReadEntities(Stream reader, ISnapshotLoader<TEntityKey> loader)
        {
            var entityCount = MessagePackSerializer.Deserialize<int>(reader, options);
            for (int e = 0; e < entityCount; e += 1)
            {
                var key = MessagePackSerializer.Deserialize<EntityKeyData>(reader, options);
                loader.OnEntity(loader.Map(key));
            }
        }
    }
}