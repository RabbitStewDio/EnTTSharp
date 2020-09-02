using System.Linq;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization.Binary
{
    public static class BinaryArchiveWriterExtensions
    {
        public static SnapshotView<TEntityKey> WriteAll<TEntityKey>(this SnapshotView<TEntityKey> v,
                                                                    BinaryArchiveWriter<TEntityKey> output)
            where TEntityKey : IEntityKey
        {
            v.WriteDestroyed(output);
            v.WriteEntites(output);

            var (writeComponent, writeTag) = SnapshotExtensions.ReflectWriteHooks<TEntityKey>();
            var parameters = new object[] { output };
            foreach (var r in output.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId))
            {
                writeComponent.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            foreach (var r in output.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId))
            {
                writeTag.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }
            return v;
        }

    }
}