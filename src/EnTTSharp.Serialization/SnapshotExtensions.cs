using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public static class SnapshotExtensions
    {
        public static SnapshotView CreateSnapshot(this EntityRegistry r)
        {
            return new SnapshotView(r);
        }

        public static AsyncSnapshotView CreateAsyncSnapshot(this EntityRegistry r)
        {
            return new AsyncSnapshotView(r);
        }

        public static SnapshotLoader CreateLoader(this EntityRegistry reg)
        {
            return new SnapshotLoader(reg);
        }

        public static SnapshotLoader CreateContinuousLoader(this EntityRegistry reg)
        {
            return new ContinuousSnapshotLoader(reg);
        }
    }
}