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
            var componentHandlers = output.Registry.Handlers.Where(e => !e.Tag).OrderBy(e => e.TypeId).ToList();
            foreach (var r in componentHandlers)
            {
                writeComponent.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            var tagHandlers = output.Registry.Handlers.Where(e => e.Tag).OrderBy(e => e.TypeId).ToList();
            foreach (var r in tagHandlers)
            {
                writeTag.MakeGenericMethod(r.TargetType).Invoke(v, parameters);
            }

            v.WriteEndOfFrame(output, false);
            return v;
        }

    }
}