using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public static class SnapshotExtensions
    {
        public static ISnapshotLoader CreateLoader(this EntityRegistry reg)
        {
            return new SnapshotLoader(reg);
        }

        public static ISnapshotLoader CreateContinuousLoader(this EntityRegistry reg)
        {
            return new ContinuousSnapshotLoader(reg);
        }
    }
}