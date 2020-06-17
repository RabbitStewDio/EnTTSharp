using System;
using System.Collections;
using System.Collections.Generic;

namespace EnttSharp.Entities.Helpers
{
    public struct PredicateEnumerator<TValue> : IEnumerator<TValue>
    {
        readonly IEnumerable<TValue> contents;
        readonly Func<TValue, bool> predicate;
        IEnumerator<TValue> enumerator;

        internal PredicateEnumerator(IEnumerable<TValue> widget,
                                     Func<TValue, bool> predicate) : this()
        {
            this.contents = widget ?? throw new ArgumentNullException();
            this.predicate = predicate;
            this.enumerator = widget.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            while (enumerator.MoveNext())
            {
                var entity = enumerator.Current;
                if (predicate(entity))
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            enumerator = contents.GetEnumerator();
        }

        object IEnumerator.Current => Current;

        public TValue Current
        {
            get
            {
                return enumerator.Current;
            }
        }
    }

    public struct MappingEnumerator<TValue, TSource, TEnumerator> : IEnumerator<TValue> where TEnumerator : IEnumerator<TSource>
    {
        // Making this field readonly breaks the enumerator as it seem to prevent state updates in the parent instance.
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        TEnumerator parent;
        readonly Func<TSource, TValue> mapper;

        public MappingEnumerator(TEnumerator parent, Func<TSource, TValue> mapper)
        {
            this.parent = parent;
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public void Dispose()
        {
            parent.Dispose();
        }

        public bool MoveNext()
        {
            return parent.MoveNext();
        }

        public void Reset()
        {
            parent.Reset();
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public TValue Current
        {
            get
            {
                var current = parent.Current;
                return mapper(current);
            }
        }
    }

    public static class EnumeratorExtensions
    {
        public static MappingEnumerator<TValue, TSource, TEnumerator> Map<TValue, TSource, TEnumerator>(this TEnumerator parent, Func<TSource, TValue> mapper)
            where TEnumerator : IEnumerator<TSource>
        {
            return new MappingEnumerator<TValue, TSource, TEnumerator>(parent, mapper);
        }
    }
}