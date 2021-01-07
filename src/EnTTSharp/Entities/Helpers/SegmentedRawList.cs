using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EnTTSharp.Entities.Helpers
{
    public class SegmentedRawList<T> : IReadOnlyList<T>
    {
        static int SegmentBits(int raw)
        {
            var exp = (int) Math.Ceiling(Math.Log(raw) / Math.Log(2));
            exp = Math.Min(Math.Max(2, exp), 31);
            return exp;
        }

        static readonly T[][] Empty = new T[0][];
        readonly int segmentBitShift;
        readonly int segmentMask;
        readonly int segmentSize;
        
        T[][] data;
        int version;

        public SegmentedRawList(int segmentSize = 256,
                                int initialSize = 10)
        {
            this.segmentBitShift = SegmentBits(segmentSize);
            this.segmentSize = (1 << segmentBitShift);
            this.segmentMask = segmentSize - 1;
            
            data = Empty;
            EnsureCapacity(initialSize);
        }
        
        public int Capacity
        {
            get
            {
                return data.Length * segmentSize;
            }
        }

        public T[][] UnsafeData => data;
        
        public void StoreAt(int index, in T input)
        {
            EnsureCapacity(index + 1);
            var segmentIdx = index >> segmentBitShift;
            try
            {
                var segment = this.data[segmentIdx];
                if (segment == null)
                {
                    segment = new T[segmentSize];
                    this.data[segmentIdx] = segment;
                }

                var dataIdx = index & segmentMask;
                segment[dataIdx] = input;
                this.Count = Math.Max(index + 1, Count);
                this.version += 1;
            }
            catch (IndexOutOfRangeException r)
            {
                Console.WriteLine("SEgmentIndex: " + segmentIdx);
                throw;
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

        public ref T TryGetRef(int index, ref T defaultValue, out bool success)
        {
            if (index < 0 || index >= Count)
            {
                success = false;
                return ref defaultValue;
            }

            var segmentIdx = index >> segmentBitShift;
            var segment = this.data[segmentIdx];
            if (segment == null)
            {
                success = false;
                return ref defaultValue;
            }
            
            var dataIdx = index & segmentMask;
            success = true;
            return ref segment[dataIdx];
        }

        public int Count
        {
            get;
            private set;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, $"Index {index} must be positive and less than {Count}");
                }
                
                var segmentIdx = index >> segmentBitShift;
                var segment = this.data[segmentIdx];
                if (segment == null)
                {
                    return default;
                }
                
                var dataIdx = index & segmentMask;
                return segment[dataIdx];
            }
            set
            {
                StoreAt(index, in value);
            }
        }

        public void Swap(int src, int dst)
        {
            var defaultVal = default(T);
            ref var srcRef = ref TryGetRef(src, ref defaultVal, out var success1);
            ref var destRef = ref TryGetRef(dst, ref defaultVal, out var success2);
            if (!success1 || !success2)
            {
                var tmpSlow = this[src];
                this[src] = this[dst];
                this[dst] = tmpSlow;
            }
            else
            {
                var tmp = srcRef;
                srcRef = destRef;
                destRef = tmp;
            }

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

        public bool TryGet(int index, out T output)
        {
            T defaultVal = default;
            ref var retval = ref TryGetRef(index, ref defaultVal, out var success);
            if (success)
            {
                output = retval;
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
            
            var capacity = Math.Max(capacityStatic, capacityDynamic);
            var segmentsNeeded = (int) Math.Ceiling(capacity / (float) segmentSize);
            Array.Resize(ref data, segmentsNeeded);
        }

        public void Add(in T e)
        {
            EnsureCapacity(Count + 1);

            this.StoreAt(Count, e);
            this.version += 1;
        }

        public void RemoveLast()
        {
            if (Count == 0) throw new ArgumentOutOfRangeException();
            
            this[Count - 1] = default;
            this.Count -= 1;
            this.version += 1;
        }

        public struct Enumerator : IEnumerator<T>
        {
            readonly SegmentedRawList<T> contents;
            readonly int versionAtStart;
            int index;
            int segment;
            T current;

            internal Enumerator(SegmentedRawList<T> widget) : this()
            {
                this.contents = widget;
                this.versionAtStart = widget.version;
                index = -1;
                segment = -1;
                current = default;
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

                current = default;
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default;
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
    }
}