using System;
using System.Linq;
using System.Reflection;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlArchiveWriterExtensions
    {
        static (MethodInfo, MethodInfo) ReflectXmlArchiveWriterExtensions<TEntityKey>() where TEntityKey : IEntityKey
        {
            var writeComponentMethod = typeof(SnapshotView<TEntityKey>).GetMethod(nameof(SnapshotView<TEntityKey>.WriteComponent),
                                                                              BindingFlags.Instance | BindingFlags.Public,
                                                                              null,
                                                                              new[] {typeof(IEntityArchiveWriter<TEntityKey>)},
                                                                              null) ??
                throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView<TEntityKey>.WriteComponent)}");

            var writeTagMethod = typeof(SnapshotView<TEntityKey>).GetMethod(nameof(SnapshotView<TEntityKey>.WriteTag),
                                                                        BindingFlags.Instance | BindingFlags.Public,
                                                                        null,
                                                                        new[] {typeof(IEntityArchiveWriter<TEntityKey>)},
                                                                        null) ??
                throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView<TEntityKey>.WriteTag)}");

            return (writeComponentMethod, writeTagMethod);
        }

        public static SnapshotView<TEntityKey> WriteAll<TEntityKey>(this SnapshotView<TEntityKey> v, XmlArchiveWriter<TEntityKey> output) 
            where TEntityKey : IEntityKey
        {
            v.WriteDestroyed(output);
            v.WriteEntites(output);

            var (writeComponent, writeTag) = ReflectXmlArchiveWriterExtensions<TEntityKey>();
            var parameters = new object[] { output };
            var handlerRegistrations = output.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                writeComponent.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            var tagRegistrations = output.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                writeTag.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }
            return v;
        }
    }
}