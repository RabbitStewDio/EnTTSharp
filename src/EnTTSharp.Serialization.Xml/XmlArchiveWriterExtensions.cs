using System;
using System.Linq;
using System.Reflection;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlArchiveWriterExtensions
    {
        static readonly MethodInfo WriteComponentMethod;
        static readonly MethodInfo WriteTagMethod;

        static XmlArchiveWriterExtensions()
        {
            WriteComponentMethod = typeof(SnapshotView).GetMethod(nameof(SnapshotView.WriteComponent),
                                                                  BindingFlags.Instance | BindingFlags.Public,
                                                                  null,
                                                                  new[] {typeof(IEntityArchiveWriter)},
                                                                  null) ??
                throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView.WriteComponent)}");

            WriteTagMethod = typeof(SnapshotView).GetMethod(nameof(SnapshotView.WriteTag),
                                                                  BindingFlags.Instance | BindingFlags.Public,
                                                                  null,
                                                                  new[] {typeof(IEntityArchiveWriter)},
                                                                  null) ??
                throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView.WriteTag)}");

        }

        public static SnapshotView WriteAll(this SnapshotView v, XmlArchiveWriter output)
        {
            v.WriteDestroyed(output);
            v.WriteEntites(output);

            var parameters = new object[] { output };
            var handlerRegistrations = output.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in handlerRegistrations)
            {
                WriteComponentMethod.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            var tagRegistrations = output.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId);
            foreach (var r in tagRegistrations)
            {
                WriteTagMethod.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }
            return v;
        }
    }
}