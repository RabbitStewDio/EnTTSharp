using System;
using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Entities
{
    public readonly struct Unit
    {
    }
    
    [EntityKey]
    [Serializable]
    public readonly struct EntityKey : IEquatable<EntityKey>, IEntityKey
    {
        public static readonly int MaxAge = 255;
        readonly uint keyData;

        public int Key => (int)(keyData & 0xFF_FFFF) - 1;
        public byte Age => (byte)((keyData >> 24) & 0xFF);

        public EntityKey(byte age, int key)
        {
            if (key < 0) throw new ArgumentException();

            keyData = (uint)(age << 24);
            keyData |= (uint)((key + 1) & 0xFF_FFFF);
        }

        public bool IsEmpty => keyData == 0;

        public bool Equals(EntityKey other)
        {
            return keyData == other.keyData;
        }

        public bool Equals(IEntityKey obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)keyData;
            }
        }

        public static bool operator ==(EntityKey left, EntityKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityKey left, EntityKey right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"EntityKey({nameof(Key)}: {Key}, {nameof(Age)}: {Age})";
        }

        public static EntityKey Create(byte age, int id)
        {
            return new EntityKey(age, id);
        }
    }

    [EntityKey]
    [Serializable]
    public readonly struct EntityKey<TPayLoad> : IEquatable<EntityKey<TPayLoad>>, IEntityKey
    {
        readonly uint keyData;
        public readonly TPayLoad PayLoad;

        public int Key => (int)keyData & 0xFF_FFFF;
        public byte Age => (byte)((keyData >> 24) & 0xFF);

        public EntityKey(byte age, int key, TPayLoad payLoad)
        {
            if (key < 0) throw new ArgumentException();

            PayLoad = payLoad;
            keyData = (uint)(age << 24);
            keyData |= (uint)(key & 0xFF_FFFF);
        }

        public bool IsEmpty => keyData == 0;
        
        public bool Equals(EntityKey<TPayLoad> other)
        {
            return keyData == other.keyData;
        }

        public bool Equals(IEntityKey obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)keyData;
            }
        }

        public static bool operator ==(EntityKey<TPayLoad> left, EntityKey<TPayLoad> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityKey<TPayLoad> left, EntityKey<TPayLoad> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(Age)}: {Age}";
        }
    }
}