using System.Linq;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public static class XmlArchiveWriterExtensions
    {
        public static SnapshotView<TEntityKey> WriteAllAsFragment<TEntityKey>(this SnapshotView<TEntityKey> v,
                                                                              XmlArchiveWriter<TEntityKey> output)
            where TEntityKey : IEntityKey
        {
            v.WriteDestroyed(output);
            v.WriteEntites(output);

            var (writeComponent, writeTag) = SnapshotExtensions.ReflectWriteHooks<TEntityKey>();
            var parameters = new object[] {output};
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

        public static SnapshotView<TEntityKey> WriteAll<TEntityKey>(this SnapshotView<TEntityKey> v,
                                                                    XmlArchiveWriter<TEntityKey> output)
            where TEntityKey : IEntityKey
        {
            output.WriteDefaultSnapshotDocumentHeader();

            v.WriteDestroyed(output);
            v.WriteEntites(output);

            var (writeComponent, writeTag) = SnapshotExtensions.ReflectWriteHooks<TEntityKey>();
            var parameters = new object[] {output};
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

            output.WriteDefaultSnapshotDocumentFooter();
            return v;
        }
    }
}