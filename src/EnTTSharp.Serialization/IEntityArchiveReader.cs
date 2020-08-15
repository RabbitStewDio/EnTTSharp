﻿using System;
using EnttSharp.Entities;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization
{
    public interface IEntityArchiveReader<TEntityKey> 
        where TEntityKey: IEntityKey
    {
        int ReadEntityCount();
        TEntityKey ReadEntity(Func<EntityKeyData, TEntityKey> entityMapper);

        int ReadComponentCount<TComponent>();
        bool TryReadComponent<TComponent>(Func<EntityKeyData, TEntityKey> entityMapper, out TEntityKey key, out TComponent component);

        bool ReadTagFlag<TComponent>();
        bool TryReadTag<TComponent>(Func<EntityKeyData, TEntityKey> entityMapper, out TEntityKey entityKey, out TComponent component);

        int ReadDestroyedCount();
        TEntityKey ReadDestroyed(Func<EntityKeyData, TEntityKey> entityMapper);

    }
}