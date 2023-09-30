using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EnTTSharp.Entities.Helpers
{
    public class RawList<T> : IReadOnlyList<T>
    {
        T[] data;
        int version;

        public RawList(int initialSize = 10)
        {
            data = new T[Math.Max(0, initialSize)];
        }

        public int Capacity
        {
            get
            {
                return data.Length;
            }
            set
            {
                if (data.Length != value)
                {
                    Array.Resize(ref data, value);
                    version += 1;
                }
            }
        }

        public void StoreAt(int index, in T input)
        {
            EnsureCapacity(index + 1);
            
            this.data[index] = input;
            this.Count = Math.Max(index + 1, Count);
            this.version += 1;
        }

        public int Version
        {
            get { return version; }
        }

        public T[] UnsafeData => data;

        public void UnsafeSetCount(int c) => Count = c;
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ref T? TryGetRef(int index, ref T? defaultValue, out bool success)
        {
            if (index < 0 || index >= Count)
            {
                success = false;
                return ref defaultValue;
            }

            success = true;
            return ref data[index]!;
        }

        public int Count
        {
            get;
            private set;
        }

        public T this[int index]
        {
            get { return data[index]; }
            set
            {
                StoreAt(index, in value);
            }
        }

        public void Swap(int src, int dst)
        {
            (data[src], data[dst]) = (data[dst], data[src]);

            this.version += 1;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(data, 0, Count);
                Count = 0;
            }

            this.version += 1;
        }

        public ref T ReferenceOf(int index)
        {
            EnsureCapacity(index + 1);
            return ref data[index];
        }

        public bool TryGet(int index, [MaybeNullWhen(false)] out T output)
        {
            if (index >= 0 && index < Count)
            {
                output = data[index];
                return true;
            }

            output = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity(int minIndexNeeded)
        {
            if (minIndexNeeded < Capacity)
            {
                return;
            }

            var capacityDynamic = Math.Min(minIndexNeeded + 10000, Capacity * 150 / 100);
            var capacityStatic = Count + 500;
            Capacity = Math.Max(capacityStatic, capacityDynamic);
        }

        public void Add(in T e)
        {
            EnsureCapacity(Count + 1);

            this.data[Count] = e;
            Count += 1;
            this.version += 1;
        }

        public void RemoveLast()
        {
            this.data[Count - 1] = default!;
            this.Count -= 1;
            this.version += 1;
        }

        public struct Enumerator : IEnumerator<T>
        {
            readonly RawList<T> contents;
            readonly int versionAtStart;
            int index;
            T current;

            internal Enumerator(RawList<T> widget) : this()
            {
                this.contents = widget;
                this.versionAtStart = widget.version;
                index = -1;
                current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (versionAtStart != contents.version)
                {
                    throw new InvalidOperationException("Concurrent Modification of RawList while iterating.");
                }

                if (index + 1 < contents.Count)
                {
                    index += 1;
                    current = contents[index];
                    return true;
                }

                current = default!;
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default!;
            }

            object IEnumerator.Current => Current!;

            public T Current
            {
                get
                {
                    if (index < 0 || index >= contents.Count)
                    {
                        throw new InvalidOperationException();
                    }

                    return current;
                }
            }
        }
    }
}