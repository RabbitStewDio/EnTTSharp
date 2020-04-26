using System.Collections.Generic;

namespace EnttSharp.Entities
{
  public class SparseDictionary<TComponent> : SparseSet, ISortableCollection<TComponent>
  {
    readonly RawList<TComponent> instances;

    public SparseDictionary()
    {
      instances = new RawList<TComponent>();
    }

    public override int Capacity
    {
      get { return base.Capacity; }
      set
      {
        base.Capacity = value;
        instances.Capacity = value;
      }
    }

    public IEnumerable<TComponent> Instances => instances;

    public virtual void Add(EntityKey e, in TComponent component)
    {
      base.Add(e);
      instances.Add(component);
    }

    public override bool Remove(EntityKey e)
    {
      var idx = IndexOf(e);
      if (idx == -1)
      {
        return false;
      }

      instances[idx] = instances[instances.Count - 1];
      instances.RemoveLast();
      base.Remove(e);
      return true;
    }

    public TComponent this[int index]
    {
      get { return instances[index]; }
    }

    void ISortableCollection<TComponent>.Swap(int idxSrc, int idxTgt)
    {
      Swap(idxSrc, idxTgt);
    }

    protected override void Swap(int idxSrc, int idxTarget)
    {
      base.Swap(idxSrc, idxTarget);
      instances.Swap(idxSrc, idxTarget);
    }

    public override void RemoveAll()
    {
      base.RemoveAll();
      instances.Clear();
    }

    public bool TryGet(EntityKey entity, out TComponent component)
    {
      var idx = IndexOf(entity);
      if (idx >= 0)
      {
        component = instances[idx];
        return true;
      }

      component = default(TComponent);
      return false;
    }

    public virtual bool Replace(EntityKey entity, in TComponent component)
    {
      return WriteBack(entity, in component);
    }

    public virtual bool WriteBack(EntityKey entity, in TComponent component)
    {
      var idx = IndexOf(entity);
      if (idx == -1)
      {
        return false;
      }

      instances[idx] = component;
      return true;
    }
  }
}