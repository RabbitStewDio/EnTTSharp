﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace EnTTSharp.Entities.Helpers
{
    /// <summary>
    ///  Standard sparse-set implementation (see
    ///  https://www.geeksforgeeks.org/sparse-set/ )
    /// 
    /// </summary>
    public class SparseSet<TEntityKey> : IEnumerable<TEntityKey>
        where TEntityKey: IEntityKey
    {
        readonly struct ReverseEntry
        {
            const uint AgeMask = 0xFF00_0000;
            const uint KeyMask = 0x007F_FFFF;
            const uint ValidMask = 0x0080_0000;

            readonly uint rawData;
            public int DenseArrayIndex => (int)(rawData & KeyMask);
            public int Age => (int)(rawData & AgeMask) >> 24;
            public bool InUse => (rawData & ValidMask) != 0;

            public ReverseEntry(int key, byte age)
            {
                rawData = (uint)(key & KeyMask);
                rawData |= (uint)(age << 24); 
                rawData |= ValidMask;
            }

            public static ReverseEntry Unused()
            {
                return default; //new ReverseEntry(key);
            }
        }

        static readonly EqualityComparer<TEntityKey> EqualityHandler = EqualityComparer<TEntityKey>.Default; 

        /// <summary>
        ///  Contains the entites in the set in insertion order.
        /// </summary>
        readonly SegmentedRawList<TEntityKey> direct;

        /// <summary>
        ///  stores an pointer into the direct list.
        /// </summary>
        readonly RawList<ReverseEntry> reverse;

        public SparseSet()
        {
            direct = new SegmentedRawList<TEntityKey>();
            reverse = new RawList<ReverseEntry>();
        }

        public int Count => direct.Count;

        public void Add(TEntityKey e)
        {
            if (Contains(e))
            {
                throw new ArgumentException("Entity already exists in this collection");
            }

            var pos = e.Key;
            reverse.StoreAt(pos, new ReverseEntry(direct.Count, e.Age));
            direct.Add(e);
        }

        public virtual bool Remove(TEntityKey e)
        {
            return RemoveEntry(e) != -1;
        }

        protected int RemoveEntry(in TEntityKey e)
        {
            var reverseArrayPosition = e.Key;
            if (reverseArrayPosition >= reverse.Count)
            {
                return -1;
            }

            var reverseEntry = reverse[reverseArrayPosition];
            if (!reverseEntry.InUse || e.Age != reverseEntry.Age)
            {
                return -1;
            }
            
            if (direct.Count == 0)
            {
                throw new ArgumentException("Inconsistent sparse-set detected");
            }

            var lastFilledEntry = direct[direct.Count - 1];
            var lastFilledIndex = lastFilledEntry.Key;

            var denseArrayIndex = reverseEntry.DenseArrayIndex;
            reverse[lastFilledIndex] = new ReverseEntry(denseArrayIndex, lastFilledEntry.Age);
            reverse[reverseArrayPosition] = ReverseEntry.Unused();

            // remove the element by filling the gap with the last entry of 
            // the direct-list into the spot that just got empty.
            // Although this reorders the elements in the list, it avoids moving all
            // elements to close the gap.
            direct[denseArrayIndex] = lastFilledEntry;
            direct.RemoveLast();
            return denseArrayIndex;
            
        }

        public TEntityKey Last => direct[direct.Count - 1];

        /// <summary>
        ///  Increases the capacity of the sparse set. This never reduces the capacity.
        /// </summary>
        /// <param name="cap"></param>
        public void Reserve(int cap)
        {
            Capacity = Math.Max(cap, Capacity);
        }

        public virtual int Capacity
        {
            get { return reverse.Capacity; }
            set { reverse.Capacity = value; }
        }

        public int Extent
        {
            get { return reverse.Count; }
        }

        public bool Contains(TEntityKey entity)
        {
            var pos = entity.Key;
            if (pos < reverse.Count)
            {
                var rk = reverse[pos];
                if (!rk.InUse || entity.Age != rk.Age)
                {
                    return false;
                }
                
                return true;
            }

            return false;
        }

        public int IndexOf(TEntityKey entity)
        {
            var pos = entity.Key;
            if (pos >= reverse.Count)
            {
                return -1;
            }

            var reverseEntry = reverse[pos];
            if (!reverseEntry.InUse || entity.Age != reverseEntry.Age)
            {
                return -1;
            }

            return reverseEntry.DenseArrayIndex;
        }

        public void Reset(TEntityKey entity)
        {
            // same as remove, but do not fail if not there
            if (Contains(entity))
            {
                Remove(entity);
            }
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public SegmentedRawList<TEntityKey>.Enumerator GetEnumerator()
        {
            return direct.GetEnumerator();
        }

        protected virtual void Swap(int idxSrc, int idxTarget)
        {
            var reverseSrc = direct[idxSrc].Key;
            var reverseTgt = direct[idxTarget].Key;
            direct.Swap(idxSrc, idxTarget);
            reverse.Swap(reverseSrc, reverseTgt);
        }

        public void Respect(IEnumerable<TEntityKey> other)
        {
            // where do we drop items that have been moved out of the way ..
            var targetPosition = direct.Count - 1;
            foreach (var otherEntity in other)
            {
                var posLocal = IndexOf(otherEntity);
                if (posLocal != -1)
                {
                    if (EqualityHandler.Equals(otherEntity, direct[targetPosition]))
                    {
                        Swap(targetPosition, posLocal);
                    }

                    targetPosition -= 1;
                }
            }
        }

        public virtual void RemoveAll()
        {
            reverse.Clear();
            direct.Clear();
        }

        public static SparseSet<TEntityKey> CreateFrom<TEnumerator>(TEnumerator members)
            where TEnumerator : IEnumerator<TEntityKey>
        {
            var set = new SparseSet<TEntityKey>();
            while (members.MoveNext())
            {
                set.Add(members.Current!);
            }

            return set;
        }

        public void CopyTo(RawList<TEntityKey> entites)
        {
            entites.Capacity = Math.Max(entites.Capacity, Count);
            entites.Clear();

            if (direct.Count == 0)
            {
                return;
            }

            var count = 0;
            var toBeCopied = direct.Count;
            var unsafeData = direct.UnsafeData;
            var data = entites.UnsafeData;
            
            for (var segmentIndex = 0; segmentIndex < unsafeData.Length; segmentIndex++)
            {
                var segment = unsafeData[segmentIndex];
                if (segment == null)
                {
                    continue;
                }
                
                for (var dataIndex = 0; dataIndex < segment.Length; dataIndex++)
                {
                    data[count] = segment[dataIndex];
                    count += 1;
                    if (count == toBeCopied)
                    {
                        entites.UnsafeSetCount(direct.Count);
                        return;
                    }
                }
            }
        }
    }
}