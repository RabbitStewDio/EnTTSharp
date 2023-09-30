using MessagePack;
using MessagePack.Formatters;
using System;

namespace EnTTSharp.Serialization.Binary
{
    public class OptionalResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new OptionalResolver();

        OptionalResolver()
        {
        }

        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T>? Formatter;

            // generic's static constructor should be minimized for reduce type generation size!
            // use outer helper method.
            static FormatterCache()
            {
                var t = typeof(T);
                if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Optional<>))
                {
                    Formatter = null;
                    return;
                }

                var args = t.GetGenericArguments();
                var formatterType = typeof(OptionalMessagePackFormatter<>).MakeGenericType(args);
                Formatter = (IMessagePackFormatter<T>)Activator.CreateInstance(formatterType);
            }
        }
    }
}