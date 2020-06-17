using System;
using System.Linq;
using System.Reflection;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlArchiveReaderExtensions
    {
        static readonly MethodInfo ReadComponentMethod;
        static readonly MethodInfo ReadTagMethod;

        static XmlArchiveReaderExtensions()
        {
            ReadComponentMethod = typeof(SnapshotStreamReader).GetMethod(nameof(SnapshotStreamReader.ReadComponent),
                                                                         BindingFlags.Instance | BindingFlags.Public,
                                                                         null,
                                                                         new[] { typeof(IEntityArchiveReader) },
                                                                         null) ??
                                  throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotStreamReader.ReadComponent)}");

            ReadTagMethod = typeof(SnapshotStreamReader).GetMethod(nameof(SnapshotStreamReader.ReadTag),
                                                                   BindingFlags.Instance | BindingFlags.Public,
                                                                   null,
                                                                   new[] { typeof(IEntityArchiveReader) },
                                                                   null) ??
                            throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotStreamReader.ReadTag)}");

        }

        public static SnapshotStreamReader ReadAll(this SnapshotStreamReader v, XmlEntityArchiveReader output)
        {
            v.ReadDestroyed(output);
            v.ReadEntities(output);

            var parameters = new object[] { output };
            var handlerRegistrations = output.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                ReadComponentMethod.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            var tagRegistrations = output.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                ReadTagMethod.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }
            return v;
        }
    }
}