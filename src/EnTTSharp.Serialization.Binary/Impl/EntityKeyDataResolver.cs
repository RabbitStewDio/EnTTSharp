using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public class EntityKeyDataResolver : IFormatterResolver
    {
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) == typeof(EntityKeyData))
            {
                return (IMessagePackFormatter<T>)EntityKeyDataFormatter.Instance;
            }

            return null;
        }
    }
}