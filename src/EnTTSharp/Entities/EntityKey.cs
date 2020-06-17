using System;

namespace EnttSharp.Entities
{
    public interface IEntityKey: IEquatable<IEntityKey>
    {
        byte Age { get; }
        int Key { get; }
    }
    
    [Serializable]
    public readonly struct EntityKey : IEquatable<EntityKey>
    {
        readonly uint keyData;
        readonly uint extraData;

        public int Key => (int)keyData;
        public uint Extra => extraData & 0xFF_FFFF;
        public byte Age => (byte)((extraData >> 24) & 0xFF);

        public EntityKey(byte age, int key, uint extra = 0)
        {
            if (key < 0) throw new ArgumentException();

            extraData = (uint)(age << 24);
            extraData |= extra & 0xFF_FFFF;
            keyData = (uint)key;
        }

        public bool Equals(EntityKey other)
        {
            return keyData == other.keyData && extraData == other.extraData;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)keyData * 397) ^ (int)extraData;
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
            return $"{nameof(Key)}: {Key}, {nameof(Age)}: {Age}";
        }
    }
}