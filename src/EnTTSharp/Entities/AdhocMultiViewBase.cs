using System;
using EnTTSharp.Entities.Helpers;
using EnTTSharp.Entities.Pools;

namespace EnTTSharp.Entities
{
    /// <summary>
    ///  Specialisation to avoid boxing when requesting an enumerator.
    /// </summary>
    public abstract class AdhocMultiViewBase<TEntityKey> : MultiViewBase<TEntityKey, PredicateEnumerator<TEntityKey>> 
        where TEntityKey : IEntityKey
    {
        protected AdhocMultiViewBase(IEntityPoolAccess<TEntityKey> registry,
                                     params IReadOnlyPool<TEntityKey>[] entries) : base(registry, entries)
        {
        }

        public override int EstimatedSize
        {
            get
            {
                if (Sets.Count == 0)
                {
                    return 0;
                }

                var count = int.MaxValue;
                for (var index = 0; index < Sets.Count; index++)
                {
                    var set = Sets[index];
                    if (set.Count < count)
                    {
                        count = set.Count;
                    }
                }

                return count;
            }
        }

        public override void CopyTo(RawList<TEntityKey> k)
        {
            k.Clear();
            k.Capacity = Math.Max(k.Capacity, EstimatedSize);

            var s = FindMinimumEntrySet(Sets);
            s.CopyTo(k);
        }

        public override PredicateEnumerator<TEntityKey> GetEnumerator()
        {
            IReadOnlyPool<TEntityKey>? s = null;
            var count = int.MaxValue;
            for (var index = 0; index < Sets.Count; index++)
            {
                var set = Sets[index];
                if (set.Count < count)
                {
                    s = set;
                    count = s.Count;
                }
            }

            if (s == null)
            {
                throw new ArgumentException();
            }

            return new PredicateEnumerator<TEntityKey>(s, IsMemberPredicate);
        }
    }
}