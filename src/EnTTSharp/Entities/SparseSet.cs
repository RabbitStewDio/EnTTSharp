using System;
using System.Collections;
using System.Collections.Generic;

namespace EnttSharp.Entities
{
  /// <summary>
  ///  Standard sparse-set implementation (see https://www.geeksforgeeks.org/sparse-set/ )
  /// 
  /// </summary>
  public class SparseSet: IEnumerable<EntityKey>
  {
    readonly struct ReverseEntry
    {
      const int InUseFlag = 1 << 28;

      readonly int rawData;
      public int Key => (rawData & 0x0FFF_FFFF) - 1;
      public bool InUse => ((rawData >> 28) & 0xF) != 0;

      public ReverseEntry(int key)
      {
        rawData = ((key + 1) & 0x0FFF_FFFF) | InUseFlag;
      }

      ReverseEntry(ReverseEntry r)
      {
        rawData = r.Key;
      }

      public ReverseEntry MarkUnused()
      {
        return new ReverseEntry(this);
      }
    }

    /// <summary>
    ///  Contains the entites in the set in insertion order.
    /// </summary>
    readonly RawList<EntityKey> direct;
    /// <summary>
    ///  stores an pointer into the direct list.
    /// </summary>
    readonly RawList<ReverseEntry> reverse;

    public SparseSet()
    {
      direct = new RawList<EntityKey>();
      reverse = new RawList<ReverseEntry>();
    }

    public int Count => direct.Count;

    public void Add(EntityKey e)
    {
      if (Contains(e))
      {
        throw new ArgumentException("Entity already exists in this collection");
      }

      var pos = e.Key;
      reverse.StoreAt(pos, new ReverseEntry(direct.Count));
      direct.Add(e);
    }

    public virtual bool Remove(EntityKey e)
    {
      if (!Contains(e))
      {
        return false;
      }

      var entt = e.Key;
      var lastEntry = direct[direct.Count - 1];
      var lastIndex = lastEntry.Key;

      var rentry = reverse[entt];
      reverse[entt] = rentry.MarkUnused();

      // remove the element by filling the gap with the last entry of 
      // the direct-list into the spot that just got empty.
      // Although this reorders the elements in the list, it avoids moving all
      // elements to close the gap. 
      reverse[lastIndex] = new ReverseEntry(rentry.Key);
      direct[rentry.Key] = lastEntry;
      direct.RemoveLast();
      return true;
    }

    protected EntityKey Last => direct[direct.Count - 1];

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
      get { return direct.Capacity; }
      set { direct.Capacity = value; }
    }

    public int Extent
    {
      get { return reverse.Count; }
    }

    public bool Contains(EntityKey entity)
    {
      var pos = entity.Key;
      if (pos < reverse.Count)
      {
        var rk = reverse[pos];
        return rk.InUse && direct[rk.Key] == entity; 
      }

      return false;
    }

    public int IndexOf(EntityKey entity)
    {
      var pos = entity.Key;
      if (pos < reverse.Count)
      {
        var entityKey = reverse[pos];
        if (entityKey.InUse && direct[entityKey.Key] == entity)
        {
          return entityKey.Key;
        }
      }

      return -1;
    }

    public void Reset(EntityKey entity)
    {
      // same as remove, but do not fail if not there
      if (Contains(entity))
      {
        Remove(entity);
      }
    }

    IEnumerator<EntityKey> IEnumerable<EntityKey>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public RawList<EntityKey>.Enumerator GetEnumerator()
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

    public void Respect(IEnumerable<EntityKey> other)
    {
      // where do we drop items that have been moved out of the way ..
      var targetPosition = direct.Count - 1;
      foreach (var otherEntity in other)
      {
        var posLocal = IndexOf(otherEntity);
        if (posLocal != -1)
        {
          if (otherEntity != direct[targetPosition])
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

    public static SparseSet CreateFrom<TEnumerator>(TEnumerator members) 
        where TEnumerator: IEnumerator<EntityKey>
    {
      var set = new SparseSet();
      while (members.MoveNext())
      {
        set.Add(members.Current);
      }

      return set;
    }
  }
}