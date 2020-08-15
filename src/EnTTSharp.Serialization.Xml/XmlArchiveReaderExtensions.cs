using System;
using System.Linq;
using System.Reflection;
using EnttSharp.Entities;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlArchiveReaderExtensions
    {
        static (MethodInfo, MethodInfo) ReflectXmlArchiveReaderExtensions<TEntityKey>() where TEntityKey : IEntityKey
        {
            var readComponentMethod = typeof(SnapshotStreamReader<TEntityKey>).GetMethod(nameof(SnapshotStreamReader<TEntityKey>.ReadComponent),
                                                                                        BindingFlags.Instance | BindingFlags.Public,
                                                                                        null,
                                                                                        new[] { typeof(IEntityArchiveReader<TEntityKey>) },
                                                                                        null) ??
                                  throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotStreamReader<TEntityKey>.ReadComponent)}");

            var readTagMethod = typeof(SnapshotStreamReader<TEntityKey>).GetMethod(nameof(SnapshotStreamReader<TEntityKey>.ReadTag),
                                                                                   BindingFlags.Instance | BindingFlags.Public,
                                                                                   null,
                                                                                   new[] { typeof(IEntityArchiveReader<TEntityKey>) },
                                                                                   null) ??
                            throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotStreamReader<TEntityKey>.ReadTag)}");
            return (readComponentMethod, readTagMethod);
        }

        public static SnapshotStreamReader<TEntityKey> ReadAll<TEntityKey>(this SnapshotStreamReader<TEntityKey> v, XmlEntityArchiveReader<TEntityKey> output) 
            where TEntityKey : IEntityKey
        {
            v.ReadDestroyed(output);
            v.ReadEntities(output);

            var (readComponent, readTag) = ReflectXmlArchiveReaderExtensions<TEntityKey>();
            var parameters = new object[] { output };
            var handlerRegistrations = output.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                readComponent.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            var tagRegistrations = output.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                readTag.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }
            return v;
        }
    }
}