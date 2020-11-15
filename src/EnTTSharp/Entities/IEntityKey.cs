using System;

namespace EnTTSharp.Entities
{
    public interface IEntityKey: IEquatable<IEntityKey>
    {
        byte Age { get; }
        int Key { get; }
        
        int GetHashCode();
        bool IsEmpty { get; }
    }
}