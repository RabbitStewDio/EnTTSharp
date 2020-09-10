﻿using System;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization
{
    public delegate TEntityKey EntityKeyMapper<TEntityKey>(EntityKeyData data);

    [Serializable]
    [DataContract]
    public struct EntityKeyData : IEquatable<EntityKeyData>
    {
        [DataMember(Name = "key", Order = 0)]
        public int Key;

        [DataMember(Name = "age", Order = 1)]
        public byte Age;

        public EntityKeyData(byte age, int key)
        {
            Age = age;
            Key = key;
        }

        public override string ToString()
        {
            return $"{nameof(Age)}: {Age}, {nameof(Key)}: {Key}";
        }

        public bool Equals(EntityKeyData other)
        {
            return Age == other.Age && Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityKeyData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Age * 397) ^ Key;
            }
        }

        public static bool operator ==(EntityKeyData left, EntityKeyData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityKeyData left, EntityKeyData right)
        {
            return !left.Equals(right);
        }
    }
}