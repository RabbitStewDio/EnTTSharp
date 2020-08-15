﻿using System;
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

        protected override int EstimatedSize
        {
            get
            {
                if (Sets.Count == 0)
                {
                    return 0;
                }

                var count = int.MaxValue;
                foreach (var set in Sets)
                {
                    if (set.Count < count)
                    {
                        count = set.Count;
                    }
                }

                return count;
            }
        }

        public override PredicateEnumerator<TEntityKey> GetEnumerator()
        {
            IReadOnlyPool<TEntityKey> s = null;
            var count = int.MaxValue;
            foreach (var set in Sets)
            {
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