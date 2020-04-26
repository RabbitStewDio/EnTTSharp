using System;
using System.Diagnostics;

namespace EnttSharp.Entities
{
    public static class LogProvider
    {
        static volatile LogFactoryDelegate factoryMethod;

        public interface ILogAdapter
        {
            void Log(TraceEventType eventType, int eventId, string message);
        }

        public delegate ILogAdapter LogFactoryDelegate(Type t);

        public static LogFactoryDelegate FactoryMethod
        {
            get => factoryMethod;
            set => factoryMethod = value;
        }

        public static ILogAdapter Create(Type t)
        {
            return FactoryMethod?.Invoke(t) ?? new DefaultLogAdapter(t);
        }

        internal static string NameWithoutGenerics(Type t)
        {
            var name = t.FullName ?? t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        class DefaultLogAdapter : ILogAdapter
        {
            readonly string name;

            public DefaultLogAdapter(Type t)
            {
                name = NameWithoutGenerics(t);
            }

            public void Log(TraceEventType eventType, int eventId, string message)
            {
                Debug.WriteLine($"[{eventType}:{eventId}] {name} - {message}");
            }
        }
    }
}