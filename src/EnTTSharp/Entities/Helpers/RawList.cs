using System;
using System.Collections;
using System.Collections.Generic;

namespace EnttSharp.Entities.Helpers
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

        public void StoreAt(int index, T element)
        {
            if (Count <= index)
            {
                Capacity = Math.Max(Capacity, index + 1);
                var def = default(T);
                while (Count <= index)
                {
                    Add(def);
                }
            }

            this[index] = element;
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
                current = default(T);
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

                current = default(T);
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default(T);
            }

            object IEnumerator.Current => Current;

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

        public int Version
        {
            get { return version; }
        }

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
                data[index] = value;
                this.version += 1;
            }
        }

        public void Swap(int src, int dst)
        {
            var tmp = data[src];
            data[src] = data[dst];
            data[dst] = tmp;

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

        public bool TryGet(int index, ref T output)
        {
            if (index >= 0 && index < Count)
            {
                output = data[index];
                return true;
            }

            return false;
        }

        public void Add(in T e)
        {
            if ((Count + 1) >= Capacity)
            {
                // lets not be conservative here ..
                // resizing is expensive after all.
                Capacity = Math.Min(Capacity + 10000, Capacity * 150 / 100);
            }

            this.data[Count] = e;
            Count += 1;
            this.version += 1;
        }

        public void RemoveLast()
        {
            this.data[Count - 1] = default(T);
            this.Count -= 1;
            this.version += 1;
        }
    }
}